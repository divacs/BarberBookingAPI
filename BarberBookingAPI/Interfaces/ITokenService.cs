using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
