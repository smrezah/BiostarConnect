using System.Runtime.InteropServices;
using webRestaurantBS.Models;
using webRestaurantBS.SFBioClasses;

namespace webRestaurantBS.Services
{
    public class BioStarDeviceSession
    {
        private readonly BioStarDeviceOptions _opt;
        private readonly ILogger _logger;

        private readonly object _sync = new();
        private bool _reconnecting = false;

        private IntPtr _context = IntPtr.Zero;
        private uint _deviceId;

        private API.OnLogReceived _logCallback;
        private API.OnDeviceDisconnected _onDisconnected;
        private API.OnDeviceConnected _onConnected;

        private int _reconnectAttempt = 0;
        private const int MAX_RECONNECT_SECONDS = 60;

        public DateTime LastSeen { get; private set; } = DateTime.UtcNow;
        public string Name => _opt.Name;

        public event Action<IntPtr, uint, BS2Event> FaceEventReceived;
        public event Action<DeviceStatusDto> StatusChanged;

        public BioStarDeviceSession(BioStarDeviceOptions opt, ILogger logger)
        {
            _opt = opt;
            _logger = logger;
        }

        /* ================= PUBLIC ================= */

        public void Start()
        {
            _ = Task.Run(async () =>
            {
                await SafeInitialConnectAsync();
            });
        }

        public void Stop()
        {
            lock (_sync)
            {
                Cleanup();
            }
        }
        private async Task SafeInitialConnectAsync()
        {
            while (true)
            {
                try
                {
                    lock (_sync)
                    {
                        ConnectInternal();
                        StartMonitoringInternal();
                    }

                    // اگر وصل شد، Online اعلام می‌شود داخل OnDeviceConnected
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Initial connect failed for {Name}, retrying...",
                        _opt.Name);

                    int delay = GetReconnectDelaySeconds();
                    _reconnectAttempt++;

                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
            }
        }

        /* ================= INTERNAL ================= */

        private void ConnectInternal()
        {
            _context = API.BS2_AllocateContext();
            if (_context == IntPtr.Zero)
                throw new Exception("AllocateContext failed");

            Check(API.BS2_Initialize(_context), "BS2_Initialize");

            API.BS2_SetKeepAliveTimeout(_context, 60000);

            _onConnected = deviceId =>
            {
                lock (_sync)
                {
                    _reconnectAttempt = 0;
                    _reconnecting = false;
                    LastSeen = DateTime.UtcNow;

                    StatusChanged?.Invoke(new DeviceStatusDto
                    {
                        Device = _opt.Name,
                        Online = true,
                        LastSeen = LastSeen
                    });
                }
            };

            _onDisconnected = deviceId =>
            {
                lock (_sync)
                {
                    LastSeen = DateTime.UtcNow;

                    StatusChanged?.Invoke(new DeviceStatusDto
                    {
                        Device = _opt.Name,
                        Online = false,
                        LastSeen = LastSeen
                    });

                    if (_reconnecting) return;
                    _reconnecting = true;

                    int delay = GetReconnectDelaySeconds();
                    _reconnectAttempt++;

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                        await SafeReconnectAsync();
                    });
                }
            };

            API.BS2_SetDeviceEventListener(
                _context,
                null, null,
                _onConnected,
                _onDisconnected
            );

            IntPtr ipPtr = Marshal.StringToHGlobalAnsi(_opt.Ip);
            try
            {
                Check(API.BS2_ConnectDeviceViaIP(
                    _context,
                    ipPtr,
                    _opt.Port,
                    out _deviceId), "BS2_ConnectDeviceViaIP");
            }
            finally
            {
                Marshal.FreeHGlobal(ipPtr);
            }
        }

        private void StartMonitoringInternal()
        {
            _logCallback = OnLogReceivedSafe;

            Check(API.BS2_StartMonitoringLog(
                _context,
                _deviceId,
                _logCallback), "StartMonitoringLog");
        }

        private async Task SafeReconnectAsync()
        {
            try
            {
                lock (_sync)
                {
                    Cleanup();
                }

                await Task.Delay(1000);

                lock (_sync)
                {
                    ConnectInternal();
                    StartMonitoringInternal();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reconnect failed for {Name}", _opt.Name);
                lock (_sync) _reconnecting = false;
            }
        }

        private void Cleanup()
        {
            try
            {
                if (_context != IntPtr.Zero)
                {
                    API.BS2_StopMonitoringLog(_context, _deviceId);
                    API.BS2_ReleaseContext(_context);
                    _context = IntPtr.Zero;
                }
            }
            catch { }
        }

        /* ================= CALLBACK SAFE ================= */

        private void OnLogReceivedSafe(uint deviceId, IntPtr logPtr)
        {
            try
            {
                var log = Marshal.PtrToStructure<BS2Event>(logPtr);
                LastSeen = DateTime.UtcNow;

                if (IsFaceEvent(log.code))
                {
                    FaceEventReceived?.Invoke(_context, deviceId, log);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnLogReceived failed for {Name}", _opt.Name);
            }
        }

        /* ================= HELPERS ================= */

        private bool IsFaceEvent(ushort code)
        {
            return
                code == (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE ||
                code == (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_FACE;
        }

        private void Check(int result, string action)
        {
            if (result < 0)
                throw new Exception($"{action} failed: {(BS2ErrorCode)result}");
        }

        private int GetReconnectDelaySeconds()
        {
            //int delay = (int)Math.Pow(2, _reconnectAttempt) * 5;
            int delay = 3;
            return Math.Min(delay, MAX_RECONNECT_SECONDS);
        }
    }
}
