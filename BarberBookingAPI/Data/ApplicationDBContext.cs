using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BarberBookingAPI.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser> // Inherits from IdentityDbContext to support ASP.NET Core Identity
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<BarberService> BarberServices { get; set; } // DbSet for barber services
        public DbSet<Appointment> Appointments { get; set; } // DbSet for appointments
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }

}
