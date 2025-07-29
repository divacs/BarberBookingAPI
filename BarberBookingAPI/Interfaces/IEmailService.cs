namespace BarberBookingAPI.Interfaces
{
    public interface IEmailService
    {
        /// Sends a confirmation email for a new appointment
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
