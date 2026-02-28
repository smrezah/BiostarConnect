using System.ComponentModel.DataAnnotations;

namespace webRestaurantBS.Models
{
    public class TbRole
    {
        [Key]
        public long RoleId { get; set; }

        [Display(Name = "نام نقش")]
        public string RoleName { get; set; }

        [Display(Name = "توضیحات نقش")]
        public string RoleCaption { get; set; }

        [Display(Name = "وضعیت")]
        public Status Status { get; set; } = Status.Active;

        [Display(Name = "تاریخ ثبت")]
        public DateTime EnDate { get; set; }

        public virtual ICollection<TbUsers> ChildrenUser { get; set; }
        public virtual ICollection<TbPermissionRole> ChildrenPermissionRole { get; set; }
    }
}
