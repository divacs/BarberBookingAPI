using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Helppers;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Mapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IEmailService _emailService;
        public AppointmentController(ApplicationDBContext context, IAppointmentRepository appointmentRepo, IEmailService emailService)
        {
            _context = context;
            _appointmentRepo = appointmentRepo;
            _emailService = emailService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var appointment = await _appointmentRepo.GetAllAsnc();
            var appointmentDto = appointment.Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("pagination")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var appointment = await _appointmentRepo.GetAllAsnc(query.PageNumber, query.PageSize); 
            var appointmentDto = appointment.Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto appointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointmentModel = appointmentDto.ToAppointmentFromCreateDto();

            await _appointmentRepo.CreateAsync(appointmentModel);

            var user = await _context.Users.FindAsync(appointmentModel.ApplicationUserId);
            if (user != null)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Appointment Confirmation",
                    $"Dear {user.UserName}, you have successfully scheduled an appointment for {appointmentModel.StartTime}."
                );
            }

            return CreatedAtAction(nameof(GetById), new { id = appointmentModel.Id }, appointmentModel.ToAppointmentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
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
       
            return Ok(existingAppointment.ToAppointmentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
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
