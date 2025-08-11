using System.ComponentModel.DataAnnotations;

namespace BarberBookingAPI.DTOs.Account
{
    public class AssignRoleDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
