using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using webRestaurantBS.SFBioClasses;

namespace webRestaurantBS.Services
{
    public class BioStarUserResolver
    {
        private readonly ILogger<BioStarUserResolver> _logger;

        // Cache: userId -> userName
        private readonly ConcurrentDictionary<string, string> _cache = new(); 
        public event Action<string, string>? UserResolved;

        // Queue برای Resolve async
        private readonly BlockingCollection<(IntPtr context, uint deviceId, string userId)> _queue
            = new();

        public BioStarUserResolver(ILogger<BioStarUserResolver> logger)
        {
            _logger = logger;

            // Worker background
            Task.Factory.StartNew(
                ProcessQueue,
                TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// اگر در Cache باشد فوری برمی‌گرداند
        /// اگر نباشد، Resolve را Async می‌کند
        /// </summary>
        public string GetOrResolve(IntPtr context, uint deviceId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return "";

            if (_cache.TryGetValue(userId, out var name))
                return name;

            // هنوز نداریم → Async resolve
            _queue.TryAdd((context, deviceId, userId));

            // فعلاً خالی برگردان
            return "";
        }

        private void ProcessQueue()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                try
                {
                    ResolveUserName(item.context, item.deviceId, item.userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User resolve failed");
                }
            }
        }

        private void ResolveUserName(IntPtr context, uint deviceId, string userId)
        {
            if (_cache.ContainsKey(userId))
                return;

            IntPtr uidPtr = Marshal.StringToHGlobalAnsi(userId);

            try
            {
                BS2UserBlob[] users = new BS2UserBlob[1];

                int result = API.BS2_GetUserInfos(
                    context,
                    deviceId,
                    uidPtr,
                    1,
                    users
                );

                if (result < 0)
                {
                    _logger.LogWarning("GetUserInfos failed for {UserId}", userId);
                    return;
                }

                string userName = Encoding.UTF8
                    .GetString(users[0].name)
                    .TrimEnd('\0');

                _cache.TryAdd(userId, userName);

                _logger.LogInformation("User cached: {UserId} -> {UserName}", userId, userName);
                UserResolved?.Invoke(userId, userName);
            }
            finally
            {
                Marshal.FreeHGlobal(uidPtr);
            }
        }
    }
}
