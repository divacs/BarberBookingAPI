using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.BarberService;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using BarberBookingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var barberService = await _barberServiceRepo.GetByIdAsync(id);

            if (barberService == null)
            {
                return NotFound();
            }

            return Ok(barberService.ToBarberServiceDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBarberServiceRequestDto barberServiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var barberServiceModel = barberServiceDto.ToBarberServiceFromCreateDto();

            await _barberServiceRepo.CreateBarberServiceAsync(barberServiceModel);

            return CreatedAtAction(nameof(GetById), new { id = barberServiceModel.Id }, barberServiceModel.ToBarberServiceDto());
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DTOs.BarberService.UpdateBarberServiceRequestDto barberServiceDto)
        {
            if (barberServiceDto == null)
            {
                return BadRequest("Barber Service data is required.");
            }
            //var existingBarberService = await _barberServiceRepo.UpdateAsync(id,)
            var existingBarberService = await _barberServiceRepo.UpdateAsync(id, barberServiceDto);
            if (existingBarberService == null)
            {
                return NotFound();
            }

            return Ok(existingBarberService.ToBarberServiceDto());
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var barberService = await _barberServiceRepo.DeleteAsync(id);
            if (barberService == null)
            {
                return NotFound();
            }
   
            return NoContent();
        }
    }
}
