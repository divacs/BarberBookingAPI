namespace BarberBookingAPI.Models
{
    public class BarberService
    {
        public int Id { get; set; } // Unique identifier for the barber service, using a string type
        public string Name { get; set; } = string.Empty; // Name of the barber service
        public int Duration { get; set; } // Duration of the service in minutes
        public int Price { get; set; } // Price of the service in the smallest currency unit (e.g., rsd)

        public List<Appointment> Appointments { get; set; } // Navigation property for appointments

    }
}
