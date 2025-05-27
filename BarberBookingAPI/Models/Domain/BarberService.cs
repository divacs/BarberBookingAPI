namespace BarberBookingAPI.Models.Domain
{
    public class BarberService
    {
        public string Id { get; set; } // Unique identifier for the barber service, using a string type
        public string Name { get; set; } // Name of the barber service
        public int Duration { get; set; } // Duration of the service in minutes
        public int Price { get; set; } // Price of the service in the smallest currency unit (e.g., rsd)
        public ICollection<Appointment> Appointments { get; set; } // Navigation property for appointments

    }
}
