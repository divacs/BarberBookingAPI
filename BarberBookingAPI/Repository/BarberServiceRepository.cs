using BarberBookingAPI.Data;
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
        public async Task<List<BarberService>> GetAllAsync()
        {
            return await _context.BarberServices.ToListAsync();
        }
    }
}
