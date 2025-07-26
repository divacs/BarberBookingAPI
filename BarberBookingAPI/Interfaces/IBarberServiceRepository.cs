using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IBarberServiceRepository
    {
        Task<List<BarberService>> GetAllAsync();
        Task<BarberService?> GetByIdAsync(int id);
        Task<BarberService> CreateBarberService(BarberService barberService);
        Task<BarberService?> UpdateByIdAsync(int id, BarberService barberService);
        Task<BarberService?> DeleteByServiceAsync(int id);
    }
}
