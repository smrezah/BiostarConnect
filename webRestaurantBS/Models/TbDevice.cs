
using System.ComponentModel.DataAnnotations;

namespace webRestaurantBS.Models
{
    public class TbDevice
    {
        [Key]
        public long DeviceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DeviceIdentifier { get; set; }

        [MaxLength(200)]
        public string DeviceName { get; set; }

        public Status IsActive { get; set; } = Status.Active;

        public virtual ICollection<TbDeviceAccess> ChildrenDeviceAccess { get; set; }
    }
}
