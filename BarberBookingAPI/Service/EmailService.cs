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
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"] ?? throw new ArgumentNullException("EmailSettings:From")));
            email.To.Add(MailboxAddress.Parse(toEmail ?? throw new ArgumentNullException(nameof(toEmail))));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            var allowInvalidCertificatesInDevelopment =
                _environment.IsDevelopment() &&
                bool.TryParse(_configuration["EmailSettings:AllowInvalidCertificateInDevelopment"], out var allowInvalidCertificates) &&
                allowInvalidCertificates;

            if (allowInvalidCertificatesInDevelopment)
            {
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

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
