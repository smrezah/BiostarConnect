using System.ComponentModel.DataAnnotations;

namespace webRestaurantBS.Models
{
    public class TbDeviceAccess
    {
        [Key]
        public long DeviceAccessId { get; set; }

        public long UserId { get; set; }
        public long DeviceId { get; set; }

        public bool HasAccess { get; set; } = true;

        public DateTime EnDate { get; set; }

        public virtual TbUsers User { get; set; }
        public virtual TbDevice Device { get; set; }
    }
}
