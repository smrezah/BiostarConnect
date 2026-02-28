using System.ComponentModel.DataAnnotations;

namespace webRestaurantBS.Models
{
    public class TbPermissionRole
    {
        [Key]
        public long PermissionRoleId { get; set; }

        public long RoleId { get; set; }
        public long MenuId { get; set; }

        public DateTime EnDate { get; set; }

        public virtual TbRole Role { get; set; }
        public virtual TbMenu Menu { get; set; }
    }
}
