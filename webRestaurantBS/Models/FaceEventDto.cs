namespace webRestaurantBS.Models
{
    public class FaceEventDto
    {
        public uint DeviceId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime EventTime { get; set; }
    }
}
