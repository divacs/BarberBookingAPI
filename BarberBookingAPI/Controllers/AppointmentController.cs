using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.Appointment;
using BarberBookingAPI.Helppers;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Jobs;
using BarberBookingAPI.Mapper;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberBookingAPI.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IEmailService _emailService;
        private readonly IAppointmentReminderJob _appointmentJob;
        private readonly IBarberServiceRepository _barberServiceRepo;
        public AppointmentController(IAppointmentRepository appointmentRepo, IEmailService emailService, IAppointmentReminderJob appointmentJob, IBarberServiceRepository barberServiceRepo)
        {
            _appointmentRepo = appointmentRepo;
            _emailService = emailService;
            _appointmentJob = appointmentJob;
            _barberServiceRepo = barberServiceRepo;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            var appointment = await _appointmentRepo.GetAllAsnc();
            var appointmentDto = appointment
                .Where(a => a.ApplicationUserId == currentUserId)
                .Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("pagination")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            var appointment = await _appointmentRepo.GetAllAsnc();
            var appointmentDto = appointment
                .Where(a => a.ApplicationUserId == currentUserId)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(a => a.ToAppointmentDto());

            return Ok(appointmentDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            var appointment = await _appointmentRepo.GetByIdAsync(id);

            if (appointment == null)
                return NotFound();

            // Verify ownership
            if (appointment.ApplicationUserId != currentUserId)
                return Forbid();

            return Ok(appointment.ToAppointmentDto());
        }
        [HttpGet("by-date")]
        [Authorize]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            if (date == default)
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            var appointments = await _appointmentRepo.GetByDateAsync(date);
            appointments = appointments.Where(a => a.ApplicationUserId == currentUserId);

            if (!appointments.Any())
                return NotFound($"No appointments found for {date:yyyy-MM-dd}.");

            var appointmentDtos = appointments.Select(a => a.ToAppointmentDto());

            return Ok(appointmentDtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto appointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            if (appointmentDto.Duration <= 0)
                return BadRequest("Duration must be greater than zero.");

            var endTime = appointmentDto.StartTime.AddMinutes(appointmentDto.Duration);

            // Check overlapping
            var existingAppointments = await _appointmentRepo.GetByDateAsync(appointmentDto.StartTime.Date);
            bool isOverlapping = existingAppointments.Any(a =>
                appointmentDto.StartTime < a.EndTime && endTime > a.StartTime
            );

            if (isOverlapping)
                return BadRequest("Cannot schedule the appointment because it conflicts with another booking.");

            // Map to model
            var appointmentModel = appointmentDto.ToAppointmentFromCreateDto();
            appointmentModel.ApplicationUserId = currentUserId;
            appointmentModel.Duration = appointmentDto.Duration; // Osiguravamo da se snimi trajanje

            await _appointmentRepo.CreateAsync(appointmentModel);

            // Send email
            var user = await _appointmentJob.GetUserByIdAsync(appointmentModel.ApplicationUserId);
            if (user != null)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Appointment Confirmation",
                    $"Dear {user.UserName}, you have successfully scheduled an appointment for {appointmentModel.StartTime}."
                );
            }

            // Schedule reminder
            var reminderTime = appointmentModel.StartTime.AddHours(-1);
            if (reminderTime > DateTime.UtcNow)
            {
                var jobId = BackgroundJob.Schedule<AppointmentReminderJob>(
                    job => job.SendReminderAsync(appointmentModel.Id),
                    reminderTime - DateTime.UtcNow
                );

                appointmentModel.ReminderJobId = jobId;
                appointmentModel.ReminderSent = false;

                await _appointmentRepo.UpdateReminderJobIdAsync(appointmentModel.Id, jobId);
            }

            return CreatedAtAction(nameof(GetById), new { id = appointmentModel.Id }, appointmentModel.ToAppointmentDto());
        }


        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAppointmentRequestDto appointmentDto)
        {
            if (appointmentDto == null)
                return BadRequest("Appointment data is required.");

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            // Verify ownership
            if (appointment.ApplicationUserId != currentUserId)
                return Forbid();

            if (appointmentDto.Duration <= 0)
                return BadRequest("Duration must be greater than zero.");

            var endTime = appointmentDto.StartTime.AddMinutes(appointmentDto.Duration);

            // Check overlapping excluding current
            var existingAppointments = await _appointmentRepo.GetByDateAsync(appointmentDto.StartTime.Date);
            bool isOverlapping = existingAppointments
                .Where(a => a.Id != id)
                .Any(a => appointmentDto.StartTime < a.EndTime && endTime > a.StartTime);

            if (isOverlapping)
                return BadRequest("Cannot schedule the appointment because it conflicts with another booking.");

            // Cancel old job
            if (!string.IsNullOrEmpty(appointment.ReminderJobId))
            {
                BackgroundJob.Delete(appointment.ReminderJobId);
                appointment.ReminderJobId = null;
            }

            // Update fields
            appointment.StartTime = appointmentDto.StartTime;
            appointment.Duration = appointmentDto.Duration;
            appointment.BarberServiceId = appointmentDto.BarberServiceId;
            appointment.ReminderSent = false;

            // Schedule new reminder
            var reminderTime = appointment.StartTime.AddHours(-1);
            if (reminderTime > DateTime.UtcNow)
            {
                var jobId = BackgroundJob.Schedule<AppointmentReminderJob>(
                    job => job.SendReminderAsync(appointment.Id),
                    reminderTime - DateTime.UtcNow
                );

                appointment.ReminderJobId = jobId;
            }

            await _appointmentJob.SaveAsync(); // Ispravljeno umesto _appointmentJob.SaveAsync()

            return Ok(appointment.ToAppointmentDto());
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Missing user id claim.");

            // Verify ownership
            if (appointment.ApplicationUserId != currentUserId)
                return Forbid();

            // Cancel scheduled reminder job
            if (!string.IsNullOrEmpty(appointment.ReminderJobId))
            {
                BackgroundJob.Delete(appointment.ReminderJobId);
            }

            await _appointmentRepo.DeleteAsync(id);

            return NoContent();
        }
    }
}
