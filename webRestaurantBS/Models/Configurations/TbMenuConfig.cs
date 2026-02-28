using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webRestaurantBS.Models.Configurations
{
    public class TbMenuConfig : IEntityTypeConfiguration<TbMenu>
    {
        public void Configure(EntityTypeBuilder<TbMenu> builder)
        {
            builder.HasKey(x => x.MenuId);
            builder.Property(p => p.NameInSystem).HasMaxLength(250);
            builder.Property(p => p.DisplayName).HasMaxLength(250);
            builder.Property(p => p.Caption).HasMaxLength(500);
            builder.Property(p => p.IconName).HasMaxLength(2000);
            builder.Property(p => p.FaDate).HasMaxLength(30);

            builder
                .HasOne(d => d.Menu)
                .WithMany(p => p.ChildrenMenu)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(m => m.ChildrenPermissionRole)
                .WithOne(r => r.Menu)
                .HasForeignKey(r => r.MenuId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
