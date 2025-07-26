namespace BarberBookingAPI.DTOs.BarberService
{
    public class UpdateBarberServiceRequestDto
    {
        public string Name { get; set; } = string.Empty; // Name of the barber service
        public int Duration { get; set; } // Duration of the service in minutes
        public int Price { get; set; } // Price of the service in the smallest currency unit (e.g., rsd)
    }
}
