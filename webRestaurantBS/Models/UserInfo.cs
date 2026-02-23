namespace webRestaurantBS.Models
{
    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PersonalId { get; set; }
        public string FullName { get; set; }
        public Guid UserGUID { get; set; }
    }
}
