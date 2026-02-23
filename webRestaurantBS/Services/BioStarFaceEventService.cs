using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webRestaurantBS.SFBioClasses;
using webRestaurantBS.Models;
using webRestaurantBS.Hubs;
using System.Text;
using Microsoft.Extensions.Options;

namespace webRestaurantBS.Services
{
    public class BioStarFaceEventService : BackgroundService
    {
        private readonly Dictionary<string, DeviceStatusDto> _deviceStates = new Dictionary<string, DeviceStatusDto>();
        private readonly BioStarOptions _options;
        private readonly IHubContext<FaceEventHub> _hub;
        private readonly ILogger<BioStarFaceEventService> _logger;
        private readonly BioStarUserResolver _userResolver; 
        private readonly DeviceStateStore _deviceStateStore;

        private readonly List<BioStarDeviceSession> _sessions = new();

        public BioStarFaceEventService(
            IOptions<BioStarOptions> options,
            IHubContext<FaceEventHub> hub,
            ILogger<BioStarFaceEventService> logger,
            BioStarUserResolver userResolver,
            DeviceStateStore deviceStateStore)
        {
            _options = options.Value;
            _hub = hub;
            _logger = logger;
            _userResolver = userResolver;
            _deviceStateStore = deviceStateStore;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var dev in _options.Devices)
            {
                _deviceStateStore.Update(new DeviceStatusDto
                {
                    Device = dev.Name,
                    Online = false,
                    LastSeen = DateTime.UtcNow
                });
            }
            _userResolver.UserResolved += OnUserResolved;

            foreach (var dev in _options.Devices)
            {
                try
                {
                    var session = new BioStarDeviceSession(dev, _logger);
                    var initialStatus = new DeviceStatusDto
                    {
                        Device = dev.Name,
                        Online = false,
                        LastSeen = DateTime.UtcNow
                    };

                    lock (_deviceStates)
                    {
                        _deviceStates[dev.Name] = initialStatus;
                    }
                    session.FaceEventReceived += OnFaceEvent;
                    session.StatusChanged += OnDeviceStatusChanged;
                    session.Start();
                    _sessions.Add(session);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to start device {Name}", dev.Name);
                }
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void OnDeviceStatusChanged(DeviceStatusDto status)
        {
            _deviceStateStore.Update(status);

            _hub.Clients.All.SendAsync("OnDeviceStatus", status);
        }


        private void OnFaceEvent(IntPtr context, uint deviceId, BS2Event log)
        {
            string userId = Encoding.ASCII
                .GetString(log.userID)
                .TrimEnd('\0');

            string userName = _userResolver.GetOrResolve(
                context,
                deviceId,
                userId
            );

            var dto = new FaceEventDto
            {
                DeviceId = deviceId,
                UserId = userId,
                UserName = userName,
                EventTime = DateTimeOffset
                    .FromUnixTimeSeconds(log.dateTime)
                    .LocalDateTime
            };

            _hub.Clients.All.SendAsync("OnFaceEvent", dto);
        }

        private void OnUserResolved(string userId, string userName)
        {
            _hub.Clients.All.SendAsync("OnUserResolved", new
            {
                UserId = userId,
                UserName = userName
            });
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _userResolver.UserResolved -= OnUserResolved;

            foreach (var session in _sessions)
            {
                session.Stop();
            }

            return base.StopAsync(cancellationToken);
        }

        public List<DeviceStatusDto> GetCurrentDeviceStates()
        {
            lock (_deviceStates)
            {
                return _deviceStates.Values.ToList();
            }
        }
    }
}