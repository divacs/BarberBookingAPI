using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IAppointmentRepository _appointmentRepo;
        public AppointmentController(ApplicationDBContext context, IAppointmentRepository appointmentRepo)
        {
            _context = context;
            _appointmentRepo = appointmentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointment = await _appointmentRepo.GetAllAsnc(); 
            var appointmentDto = appointment.Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentRepo.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment.ToAppointmentDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto appointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointmentModel = appointmentDto.ToAppointmentFromCreateDto();

            await _appointmentRepo.CreateAsync(appointmentModel);

            return CreatedAtAction(nameof(GetById), new { id = appointmentModel.Id }, appointmentModel.ToAppointmentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAppointmentRequestDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is required.");
            }
            var existingAppointment = await _appointmentRepo.UpdateAsync(id, appointmentDto);
            if (existingAppointment == null)
            {
                return NotFound();
            }
            // Validate that the selected barber service exists in the database
            //if (!await _context.Users.AnyAsync(s => s.Id == appointmentDto.ApplicationUserId))
            //{
            //    return BadRequest("Selected service does not exist.");
            //}
            // Validate that the specified user exists in the database
            //if (!await _context.Users.AnyAsync(u => u.Id == appointmentDto.ApplicationUserId))
            //{
            //    return BadRequest("Selected user does not exist.");
            //}
       
            return Ok(existingAppointment.ToAppointmentDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var appointment = await _appointmentRepo.DeleteAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
