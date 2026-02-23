namespace webRestaurantBS.Models
{
    public class BioStarOptions
    {
        public List<BioStarDeviceOptions> Devices { get; set; }
        public int ReconnectIntervalSeconds { get; set; } = 10;
        public int HeartbeatSeconds { get; set; } = 30;

    }
}
