namespace webRestaurantBS.Models
{
    public class DefaultsInfo
    {
        public Dictionary<string, string> Restaurants { get; set; }
        public Dictionary<string, string> Meals { get; set; }
        public Dictionary<string, string> ServeMethods { get; set; }
        public Dictionary<string, string> Foods { get; set; }
    }
    public class Reservation
    {
        public string Oid { get; set; }
        public DateTime Date { get; set; }
        public string MealOid { get; set; }
        public string MealName { get; set; }
        public string RestaurantOid { get; set; }
        public string RestaurantName { get; set; }
        public string ServeMethodOid { get; set; }
        public string ServeMethodName { get; set; }
        public int HolidayType { get; set; }
        public bool Disable { get; set; }
    }
    public class Restaurant
    {
        public string Oid { get; set; }
        public List<Food> Foods { get; set; }
    }

    public class Food
    {
        public string Oid { get; set; }
        public List<string> ServeMethods { get; set; }
    }
}
