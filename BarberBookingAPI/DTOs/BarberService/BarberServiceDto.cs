using System.ComponentModel.DataAnnotations;

namespace BarberBookingAPI.DTOs.BarberService
{
    public class BarberServiceDto
    {
        public int Id { get; set; } // Unique identifier for the barber service, using a string type
        [Required]
        public string Name { get; set; } = string.Empty; // Name of the barber service
        [Required]
        [MinLength(5, ErrorMessage = "Name must be 3 letters or more")]
        public int Duration { get; set; } // Duration of the service in minutes
        [Required]
        public int Price { get; set; } // Price of the service in the smallest currency unit (e.g., rsd)

    }
}
