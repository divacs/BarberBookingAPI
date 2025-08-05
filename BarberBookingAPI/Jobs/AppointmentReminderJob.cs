using BarberBookingAPI.Data;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Jobs
{
    public class AppointmentReminderJob : IAppointmentReminderJob
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailService _emailService;

        public AppointmentReminderJob(ApplicationDBContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendRemindersAsync()
        {
            var now = DateTime.UtcNow;
            var oneHourFromNow = now.AddHours(1);

            var appointments = await _context.Appointments
                .Include(a => a.ApplicationUser)
                .Where(a =>
                    a.StartTime >= oneHourFromNow.AddMinutes(-1) &&
                    a.StartTime <= oneHourFromNow.AddMinutes(1) &&
                    !a.ReminderSent)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                if (appointment.ApplicationUser?.Email == null) continue;

                await _emailService.SendEmailAsync(
                    appointment.ApplicationUser.Email,
                    "Upcoming Appointment Reminder",
                    $"Reminder: Your appointment is scheduled for {appointment.StartTime}."
                );

                appointment.ReminderSent = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SendReminderAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null || appointment.ReminderSent || appointment.ApplicationUser?.Email == null)
                return;

            await _emailService.SendEmailAsync(
                appointment.ApplicationUser.Email,
                "Upcoming Appointment Reminder",
                $"Reminder: Your appointment is scheduled for {appointment.StartTime}."
            );

            appointment.ReminderSent = true;
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.ApplicationUser)
                .Include(a => a.BarberService)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

    }
}
