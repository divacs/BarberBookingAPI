using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IBarberServiceRepository
    {
        Task<List<BarberService>> GetAllAsync();
    }
}
