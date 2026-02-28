using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webRestaurantBS.Models
{
    public class TbUsers
    {
        [Key]
        public long UserId { get; set; }

        public long? ParentId { get; set; }

        public long RoleId { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "نام نام خانوادگی")]
        public string DisplayName { get; set; }

        [Display(Name = "شماره پرسنلی")]
        public string PersonalId { get; set; }

        public string AdGuid { get; set; }

        [Display(Name = "تلفن")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        public string LocationId { get; set; }

        public string ProfileImage { get; set; }

        public string GroupNameOU { get; set; }

        [NotMapped]
        public string[] ListGroupOU
        {
            get => string.IsNullOrEmpty(GroupNameOU) ? new string[] { } : GroupNameOU.Split(';');
            set => GroupNameOU = value == null ? null : string.Join(";", value);
        }

        [Display(Name = "جنسیت")]
        public Gender Gender { get; set; }

        [Display(Name = "وضعیت")]
        public Status Status { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime? EnDate { get; set; }

        [Display(Name = "تاریخ آخرین تغییرات")]
        public DateTime? LastModifyDate { get; set; }

        public string FaDate { get; set; }

        [ForeignKey("ParentId")]
        public virtual TbUsers User { get; set; }
        public virtual TbRole Role { get; set; }

        public virtual ICollection<TbUsers> ChildrenUser { get; set; }
        public virtual ICollection<TbDeviceAccess> ChildrenDeviceAccess { get; set; }
    }
}
