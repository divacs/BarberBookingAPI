using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BarberBookingAPI.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser> // Inherits from IdentityDbContext to support ASP.NET Core Identity
    {
        private const string AdminRoleId = "3bfa9a6e-4897-4395-acd7-0b1ba741e474";
        private const string UserRoleId = "d598eac7-55ec-4862-8d53-356cb52d7a64";
        private const string WorkerRoleId = "5ae5c4c0-ff98-407b-8b99-94ca20af6b2f";

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<BarberService> BarberServices { get; set; } // DbSet for barber services
        public DbSet<Appointment> Appointments { get; set; } // DbSet for appointments
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // seeding the Roles table
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = UserRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = WorkerRoleId,
                    Name = "Worker",
                    NormalizedName = "WORKER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }

}
