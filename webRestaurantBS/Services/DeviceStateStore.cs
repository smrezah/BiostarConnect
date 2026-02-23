using System.Collections.Concurrent;
using webRestaurantBS.Models;

namespace webRestaurantBS.Services
{
    public class DeviceStateStore
    {
        private readonly ConcurrentDictionary<string, DeviceStatusDto> _states
            = new ConcurrentDictionary<string, DeviceStatusDto>();

        public void Update(DeviceStatusDto status)
        {
            _states[status.Device] = status;
        }

        public DeviceStatusDto[] GetAll()
        {
            return _states.Values.ToArray();
        }
    }
}
