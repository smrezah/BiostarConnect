using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webRestaurantBS.Models.Configurations
{
    public class TbDeviceAccessConfig : IEntityTypeConfiguration<TbDeviceAccess>
    {
        public void Configure(EntityTypeBuilder<TbDeviceAccess> builder)
        {
            builder.HasKey(x => x.DeviceAccessId);

            builder.HasIndex(x => new { x.UserId, x.DeviceId }).IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(u => u.ChildrenDeviceAccess)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Device)
                .WithMany(d => d.ChildrenDeviceAccess)
                .HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
