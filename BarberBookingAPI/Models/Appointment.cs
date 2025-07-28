using System.ComponentModel.DataAnnotations;

namespace BarberBookingAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the appointment, using an integer type
        public DateTime StartTime { get; set; } // Start time of the appointment
        public DateTime EndTime { get; set; } // End time of the appointment

        public string ApplicationUserId { get; set; } = string.Empty; // Identifier for the client, typically a user ID 
        public ApplicationUser ApplicationUser { get; set; } // Navigation property for the client
        
        public int BarberServiceId { get; set; } // Identifier for the barber service
        public BarberService BarberService { get; set; } // Navigation property for the barber service
    }
}
