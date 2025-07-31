using BarberBookingAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BarberBookingAPI.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"] ?? throw new ArgumentNullException("EmailSettings:From")));
            email.To.Add(MailboxAddress.Parse(toEmail ?? throw new ArgumentNullException(nameof(toEmail))));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            // TEMPORARY: Disable SSL certificate validation (ONLY FOR TESTING)
            // DO NOT USE THIS IN PRODUCTION - it bypasses important security checks
            smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            //
            // // END TEMPORARY: Disable SSL certificate validation

            // Connect to the SMTP server using the settings from configuration
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"] ?? throw new ArgumentNullException("EmailSettings:SmtpServer"),
                int.Parse(_configuration["EmailSettings:Port"] ?? throw new ArgumentNullException("EmailSettings:Port")),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"] ?? throw new ArgumentNullException("EmailSettings:Username"),
                _configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("EmailSettings:Password"));

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
