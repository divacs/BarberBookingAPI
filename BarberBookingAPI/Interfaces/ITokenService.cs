using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
