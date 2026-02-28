using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webRestaurantBS.Models.Configurations
{
    public class TbUsersConfig : IEntityTypeConfiguration<TbUsers>
    {
        public void Configure(EntityTypeBuilder<TbUsers> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.Property(p => p.FirstName).HasMaxLength(100);
            builder.Property(p => p.LastName).HasMaxLength(100);
            builder.Property(p => p.DisplayName).HasMaxLength(500);
            builder.Property(p => p.PersonalId).HasMaxLength(11);
            builder.Property(p => p.PhoneNumber).HasMaxLength(50);
            builder.Property(p => p.Username).HasMaxLength(200);
            builder.Property(p => p.FaDate).HasMaxLength(40);

            builder
                .HasOne(d => d.User)
                .WithMany(p => p.ChildrenUser)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
