using BarberBookingAPI.Models;
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

    }
}