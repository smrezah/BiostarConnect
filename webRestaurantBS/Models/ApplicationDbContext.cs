using Microsoft.EntityFrameworkCore;
using webRestaurantBS.Models.Configurations;

namespace webRestaurantBS.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TbUsers> TbUsers { get; set; }
        public DbSet<TbRole> TbRoles { get; set; }
        public DbSet<TbMenu> TbMenus { get; set; }
        public DbSet<TbPermissionRole> TbPermissionRoles { get; set; }
        public DbSet<TbDevice> TbDevices { get; set; }
        public DbSet<TbDeviceAccess> TbDeviceAccesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TbUsersConfig());
            modelBuilder.ApplyConfiguration(new TbDeviceAccessConfig());
            modelBuilder.ApplyConfiguration(new TbPermissionRoleConfig());
            modelBuilder.ApplyConfiguration(new TbMenuConfig());
            modelBuilder.ApplyConfiguration(new TbRoleConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
