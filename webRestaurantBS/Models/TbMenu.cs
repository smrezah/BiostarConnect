using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webRestaurantBS.Models
{
    public class TbMenu
    {
        [Key]
        public long MenuId { get; set; }

        public long? ParentId { get; set; }

        [Display(Name = "نام سیستمی")]
        public string NameInSystem { get; set; }

        [Display(Name = "نام نمایشی")]
        public string DisplayName { get; set; }

        [Display(Name = "توضیحات منو")]
        public string Caption { get; set; }

        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Url { get; set; }
        public string IconName { get; set; }

        public int SortOrder { get; set; }

        public Status ShowInMenu { get; set; }
        public Status Status { get; set; }

        public DateTime EnDate { get; set; }
        public string FaDate { get; set; }


        [ForeignKey("ParentId")]
        public virtual TbMenu Menu { get; set; }

        public virtual ICollection<TbMenu> ChildrenMenu { get; set; }
        public virtual ICollection<TbPermissionRole> ChildrenPermissionRole { get; set; }
    }
}
