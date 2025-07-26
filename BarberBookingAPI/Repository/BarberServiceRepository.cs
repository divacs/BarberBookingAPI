using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.BarberService;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Repository
{
    public class BarberServiceRepository : IBarberServiceRepository
    {
        private readonly ApplicationDBContext _context;
        public BarberServiceRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<BarberService> CreateBarberService(BarberService barberServiceModel)
        {
            await _context.BarberServices.AddAsync(barberServiceModel);
            await _context.SaveChangesAsync();
            return barberServiceModel;
        }

        public async Task<BarberService?> DeleteByServiceAsync(int id)
        {
            var barberServiceModel = await _context.BarberServices.FindAsync(id);

            if (barberServiceModel == null)
            {
                return null;
            }

            _context.BarberServices.Remove(barberServiceModel);
            await _context.SaveChangesAsync();
            return barberServiceModel;
        }

        public async Task<List<BarberService>> GetAllAsync()
        {
            return await _context.BarberServices.ToListAsync();
        }

        public async Task<BarberService?> GetByIdAsync(int id)
        {
            return await _context.BarberServices.FindAsync(id);
        }

        public async Task<BarberService?> UpdateByIdAsync(int id, BarberService barberServiceModel)
        {
            var existingBarberService = await _context.BarberServices.FindAsync(id);

            existingBarberService.Name = barberServiceModel.Name;
            existingBarberService.Duration = barberServiceModel.Duration;
            existingBarberService.Price = barberServiceModel.Price;

            await _context.SaveChangesAsync();

            return existingBarberService;
        }

    }
}
