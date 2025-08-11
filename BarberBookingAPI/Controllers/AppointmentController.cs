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

            // if DUration is not sent, take Duration from BarberService
            int duration;

            duration = appointmentDto.Duration;


            // Calculate EndTime
            var endTime = appointmentDto.StartTime.AddMinutes(duration);

            // Covert DTO i model, but we have to send EndTime
            var appointmentModel = appointmentDto.ToAppointmentFromCreateDto();
            appointmentModel.EndTime = endTime;

            await _appointmentRepo.CreateAsync(appointmentModel);

            var user = await _appointmentJob.GetUserByIdAsync(appointmentModel.ApplicationUserId);
            if (user != null)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Appointment Confirmation",
                    $"Dear {user.UserName}, you have successfully scheduled an appointment for {appointmentModel.StartTime}."
                );
            }

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

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            // Cancel old job if exists
            if (!string.IsNullOrEmpty(appointment.ReminderJobId))
            {
                BackgroundJob.Delete(appointment.ReminderJobId);
                appointment.ReminderJobId = null;
            }

            // Update fields
            appointment.StartTime = appointmentDto.StartTime;
            appointment.EndTime = appointmentDto.EndTime;
            appointment.BarberServiceId = appointmentDto.BarberServiceId;
            appointment.ReminderSent = false;

            // Schedule new job
            var reminderTime = appointment.StartTime.AddHours(-1);
            if (reminderTime > DateTime.UtcNow)
            {
                var jobId = BackgroundJob.Schedule<AppointmentReminderJob>(
                    job => job.SendReminderAsync(appointment.Id),
                    reminderTime - DateTime.UtcNow
                );

                appointment.ReminderJobId = jobId;
            }

            await _appointmentJob.SaveAsync();

            return Ok(appointment.ToAppointmentDto());
        }


        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

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
