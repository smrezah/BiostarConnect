using Microsoft.AspNetCore.SignalR;
using webRestaurantBS.Services;

namespace webRestaurantBS.Hubs
{
    public class FaceEventHub : Hub
    {
        private readonly DeviceStateStore _deviceStateStore;

        public FaceEventHub(DeviceStateStore deviceStateStore)
        {
            _deviceStateStore = deviceStateStore;
        }

        public override async Task OnConnectedAsync()
        {
            var states = _deviceStateStore.GetAll();
            await Clients.Caller.SendAsync("OnInitialDeviceStatus", states);
            await base.OnConnectedAsync();
        }
    }
}
