using BarberBookingAPI.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberBookingAPI.Controllers
{
    [Route("api/TestJob")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TestController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IEmailService _emailService;
        private readonly IAppointmentReminderJob _appointmentJob;
        private readonly IWebHostEnvironment _environment;

        public TestController(IAppointmentRepository appointmentRepo, IEmailService emailService, IAppointmentReminderJob appointmentJob, IWebHostEnvironment environment)
        {
            _appointmentRepo = appointmentRepo;
            _emailService = emailService;
            _appointmentJob = appointmentJob;
            _environment = environment;
        }

        [HttpPost("test-reminder-job")]
        public async Task<IActionResult> TestReminderJob([FromServices] IAppointmentReminderJob reminderJob)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            await reminderJob.SendRemindersAsync();
            return Ok("Reminder job executed manually.");
        }

        [HttpPost("schedule-test-job")]
        public IActionResult ScheduleTestJob([FromServices] IAppointmentReminderJob reminderJob)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            var jobId = BackgroundJob.Schedule(() => reminderJob.SendRemindersAsync(), TimeSpan.FromSeconds(10));
            return Ok($"Scheduled job with ID: {jobId}");
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            string testEmail = "nenadgf@gmail.com";
            string subject = "Test Email";
            string body = "<h2>This is a test email from BarberBookingAPI.</h2><p>If you received this, email service works!</p>";

            try
            {
                await _emailService.SendEmailAsync(testEmail, subject, body);
                return Ok("✅ Test email sent successfully to sonja.divac@gmail.com.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Failed to send email: {ex.Message}");
            }
        }
        //[HttpGet("send-multiple-emails")]
        //public async Task<IActionResult> SendMultipleEmails()
        //{
        //    var recipients = new List<string>
        //    {
        //        "aleksandar.zivkovic@stech.rs",
        //        "aleksandra.carevic@stech.rs",
        //        "marija.ristic@stech.rs",
        //        "sonja.divac@yahoo.com"
        //    };

        //    string subject = "Test Email";
        //    string body = "<h2>GDE ME VODITE U UTORAK ??? </h2>";

        //    try
        //    {
        //        foreach (var email in recipients)
        //        {
        //            await _emailService.SendEmailAsync(email, subject, body);
        //        }

        //        return Ok("✅ Emails sent successfully to all recipients.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"❌ Failed to send email: {ex.Message}");
        //    }
        //}

    }
}
