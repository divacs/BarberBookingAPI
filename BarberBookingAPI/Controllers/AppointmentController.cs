using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
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
        public AppointmentController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointment = await _context.Appointments.ToListAsync();
            var appointmentDto = appointment.Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment.ToAppointmentDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is required.");
            }
            // Validate that the selected barber service exists in the database
            if (!await _context.BarberServices.AnyAsync(s => s.Id == appointmentDto.BarberServiceId))
            {
                return BadRequest("Selected service does not exist.");
            }

            // Validate that the specified user exists in the database
            if (!await _context.Users.AnyAsync(u => u.Id == appointmentDto.ApplicationUserId))
            {
                return BadRequest("Selected user does not exist.");
            }
            var appointment = appointmentDto.ToAppointmentFromCreateDto();
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment.ToAppointmentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAppointmentRequestDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is required.");
            }
            var existingAppointment = await _context.Appointments.FindAsync(id);
            if (existingAppointment == null)
            {
                return NotFound();
            }
            // Validate that the selected barber service exists in the database
            if (!await _context.BarberServices.AnyAsync(s => s.Id == appointmentDto.BarberServiceId))
            {
                return BadRequest("Selected service does not exist.");
            }
            // Validate that the specified user exists in the database
            if (!await _context.Users.AnyAsync(u => u.Id == appointmentDto.ApplicationUserId))
            {
                return BadRequest("Selected user does not exist.");
            }
            existingAppointment.StartTime = appointmentDto.StartTime;
            existingAppointment.EndTime = appointmentDto.EndTime;
            existingAppointment.ApplicationUserId = appointmentDto.ApplicationUserId;
            existingAppointment.BarberServiceId = appointmentDto.BarberServiceId;
            await _context.SaveChangesAsync();

            return Ok(existingAppointment.ToAppointmentDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
