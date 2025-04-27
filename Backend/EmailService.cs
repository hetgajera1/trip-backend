using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Backend
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                Console.WriteLine($"[EmailService] Preparing to send email to: {toEmail}");
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                // Add Reply-To and headers to help avoid spam
                email.ReplyTo.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
                email.Headers.Add("X-Priority", "1");
                email.Headers.Add("X-Mailer", "CabinBookingApp");
                email.Headers.Add("X-Company", "Cabin Booking");
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };


                using var smtp = new SmtpClient();
                Console.WriteLine($"[EmailService] Connecting to SMTP server: {_configuration["EmailSettings:SmtpHost"]}:{_configuration["EmailSettings:SmtpPort"]}");
                await smtp.ConnectAsync(_configuration["EmailSettings:SmtpHost"], int.Parse(_configuration["EmailSettings:SmtpPort"]), true);
                Console.WriteLine("[EmailService] Authenticating...");
                await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
                Console.WriteLine("[EmailService] Sending email...");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                Console.WriteLine("[EmailService] Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService][ERROR] Failed to send email: {ex.Message}\n{ex.StackTrace}");
                throw; // Optional: rethrow or handle gracefully
            }
        }
    }
}
