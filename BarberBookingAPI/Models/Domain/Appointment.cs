namespace BarberBookingAPI.Models.Domain
{
    public class Appointment
    {
        public int Id { get; set; } // Unique identifier for the appointment, using an integer type
        public DateTime StartTime { get; set; } // Start time of the appointment
        public DateTime EndTime { get; set; } // End time of the appointment
        public string ClientId { get; set; } // Identifier for the client, typically a user ID
        public ApplicationUser Client { get; set; } // Navigation property for the client
        public string WorkerId { get; set; } // Identifier for the worker (barber), typically a user ID
        public string BarberServiceId { get; set; } // Identifier for the barber service
        public BarberService BarberService { get; set; } // Navigation property for the barber service
    }
}
