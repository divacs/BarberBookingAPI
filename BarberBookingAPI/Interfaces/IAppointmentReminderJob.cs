using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IAppointmentReminderJob
    {
        Task SendRemindersAsync();
        Task SendReminderAsync(int appointmentId);
        Task<Appointment?> GetByIdAsync(int id);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task SaveAsync();

    }
}
