using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BarberBookingAPI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<List<ApplicationUser>> GetAllWorkersAsync()
        {
            return (await _userManager.GetUsersInRoleAsync("Worker")).ToList();
        }
    }
}
