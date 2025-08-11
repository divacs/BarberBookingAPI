using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<ApplicationUser>> GetAllWorkersAsync();
    }
}
