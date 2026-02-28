using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webRestaurantBS.Models.Configurations
{
    public class TbPermissionRoleConfig : IEntityTypeConfiguration<TbPermissionRole>
    {
        public void Configure(EntityTypeBuilder<TbPermissionRole> builder)
        {
            builder.HasKey(x => x.PermissionRoleId);
            builder.HasIndex(x => new { x.RoleId, x.MenuId }).IsUnique();
        }
    }
}
