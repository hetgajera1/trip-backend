using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Backend;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ContactMessageService _contactMessageService;

        public ContactController(IConfiguration configuration, ContactMessageService contactMessageService)
        {
            _configuration = configuration;
            _emailService = new EmailService(configuration);
            _contactMessageService = contactMessageService;
        }

        public class ContactRequest
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendContact([FromBody] ContactRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Message))
                return BadRequest("Name, Email, and Message are required.");

            // Save to DB
            var contactMsg = new ContactMessage
            {
                Name = req.Name,
                Email = req.Email,
                Phone = req.Phone,
                Message = req.Message,
                CreatedAt = System.DateTime.UtcNow
            };
            await _contactMessageService.CreateAsync(contactMsg);

            var subject = $"Contact Query from {req.Name} <{req.Email}>";
            var body = $@"
                <div style='font-family:sans-serif;background:#f4f6fa;padding:32px;'>
                  <div style='max-width:520px;margin:auto;background:white;border-radius:10px;box-shadow:0 2px 8px #0001;padding:32px;'>
                    <h2 style='color:#1e4e5f;margin-bottom:16px;'>New Contact Query</h2>
                    <table style='width:100%;margin-bottom:24px;'>
                      <tr><td style='font-weight:bold;width:120px;'>Name:</td><td>{req.Name}</td></tr>
                      <tr><td style='font-weight:bold;'>Email:</td><td>{req.Email}</td></tr>
                      <tr><td style='font-weight:bold;'>Phone:</td><td>{req.Phone}</td></tr>
                      <tr><td style='font-weight:bold;vertical-align:top;'>Message:</td><td><div style='padding:12px 16px;background:#f7fafc;border-radius:6px;color:#222;'>{req.Message}</div></td></tr>
                    </table>
                    <div style='font-size:0.9em;color:#888;text-align:right;'>Received {System.DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</div>
                  </div>
                </div>
            ";
            var supportEmail = _configuration["EmailSettings:SenderEmail"] ?? "het4footprint@gmail.com";
            await _emailService.SendEmailAsync(supportEmail, subject, body);
            return Ok("Your message has been sent. Thank you!");
        }
    }
}
