using Microsoft.AspNetCore.Identity;

namespace BarberBookingAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Counter { get; set; } // Used to track the number of bookings made by the user
        public int Edited { get; set; } // Used to track the number of bookings edited by the user
        public int Cancelled { get; set; } // Used to track the number of bookings cancelled by the user
        public string FullName { get; set; } = string.Empty; // Full name of the user

        public List<Appointment> Apointments { get; set; } // Navigation property for appointments
    }
}
