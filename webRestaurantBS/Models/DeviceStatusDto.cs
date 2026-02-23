namespace webRestaurantBS.Models
{
    public class DeviceStatusDto
    {
        public string Device { get; set; }
        public bool Online { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
