using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webRestaurantBS.Models.Configurations
{
    public class TbRoleConfig : IEntityTypeConfiguration<TbRole>
    {
        public void Configure(EntityTypeBuilder<TbRole> builder)
        {
            builder.HasKey(x => x.RoleId);
            builder.Property(p => p.RoleName).HasMaxLength(250);
            builder.Property(p => p.RoleCaption).HasMaxLength(500);

            builder.HasMany(x => x.ChildrenUser).WithOne(w => w.Role).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.ChildrenPermissionRole).WithOne(w => w.Role).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
