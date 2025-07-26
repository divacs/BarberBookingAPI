using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace BarberBookingAPI.Controllers
{
    [Route("api/barberService")]
    [ApiController]
    public class BarberServiceController : ControllerBase
    {
        private readonly IBarberServiceRepository _barberServiceRepo;
        public BarberServiceController(IBarberServiceRepository barberServiceRepo)
        {
            _barberServiceRepo = barberServiceRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var barberService = await _barberServiceRepo.GetAllAsync();

            var barberServiceDto = barberService.Select(b => b.ToBarberServiceDto());

            return Ok(barberService);
        }
    }
}
