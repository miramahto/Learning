using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       
        public DbSet<ApplicationsTypeModel> ApplicationsTypes { get; set; }
        public DbSet<ApplicationsRegisterModel> Applications { get; set; }
        public DbSet<UserApplicationsMappingModel> UserApplicationsMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserApplicationsMappingModel>()
                .HasOne(u => u.ApplicationsRegisterModel)
                .WithMany()
                .HasForeignKey(u => u.AppID);
        }


    }
}
