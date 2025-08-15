using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberBookingAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the appointment, using an integer type
        public DateTime StartTime { get; set; } // Start time of the appointment
        public int Duration { get; set; } // Duration in minutes, nullable
        [NotMapped]
        public DateTime EndTime => StartTime.AddMinutes(Duration); // Calculated, don't go in db
        public bool ReminderSent { get; set; } = false;
        public string? ReminderJobId { get; set; }



        public string ApplicationUserId { get; set; } = string.Empty; // Identifier for the client, typically a user ID 
        public ApplicationUser ApplicationUser { get; set; } // Navigation property for the client
        
        public int BarberServiceId { get; set; } // Identifier for the barber service
        public BarberService BarberService { get; set; } // Navigation property for the barber service
    }
}
