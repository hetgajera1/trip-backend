using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Backend;
using System.Security.Cryptography; // For code generation


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly PasswordResetService _resetService;

        public AuthController(UserService userService, IConfiguration configuration, PasswordResetService resetService)
        {
            _userService = userService;
            _configuration = configuration;
            _emailService = new EmailService(configuration);
            _resetService = resetService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Log the received user object
            Console.WriteLine($"Received: Name={user?.Name}, Email={user?.Email}, Password={user?.Password}");
            if (!ModelState.IsValid)
            {
                // Log model state errors
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"ModelState error for {key}: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }
            var exist = await _userService.GetByEmailAsync(user.Email);
            if (exist != null) return BadRequest("User already exists");
            user.MemberSince = DateTime.UtcNow.ToString("yyyy-MM-dd");
            await _userService.CreateAsync(user);
            // Send welcome email
            await _emailService.SendEmailAsync(user.Email, "Welcome to Cabin Booking!", $@"
                <div style='background:#f8fafc;padding:32px;font-family:sans-serif;'>
                  <div style='max-width:480px;margin:auto;background:white;border-radius:12px;box-shadow:0 2px 8px #0001;padding:32px;'>
                    <div style='text-align:center;margin-bottom:16px;'>
                      <img src='https://cdn-icons-png.flaticon.com/512/684/684908.png' alt='Cabin Booking' width='64' style='margin-bottom:8px;'>
                      <h1 style='color:#2d3748;margin:0;'>Welcome, {user.Name}!</h1>
                    </div>
                    <p style='font-size:1.1em;color:#444;'>Thank you for registering with <b>Cabin Booking</b>! Your adventure begins here.</p>
                    <ul style='color:#2d3748;font-size:1em;margin:16px 0 24px 20px;'>
                      <li>🌲 Explore unique cabins</li>
                      <li>🛏️ Book your favorite stays</li>
                      <li>⭐ Enjoy exclusive member perks</li>
                    </ul>
                    <a href='https://your-cabin-booking-site.com' style='display:inline-block;padding:12px 28px;background:#2563eb;color:white;text-decoration:none;border-radius:6px;font-weight:bold;font-size:1.1em;'>Browse Cabins</a>
                    <p style='margin-top:32px;font-size:0.95em;color:#888;'>If you have any questions, reply to this email or contact our support team.<br><br>Happy travels!<br>— The Cabin Booking Team</p>
                  </div>
                </div>
            ");
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userService.GetByEmailAsync(req.Email);
            if (user == null || user.Password != req.Password)
                return Unauthorized("Invalid credentials");
            return Ok(user);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, User userIn)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            await _userService.UpdateAsync(id, userIn);
            return NoContent();
        }
        [HttpPost("request-password-code")]
        public async Task<IActionResult> RequestPasswordCode([FromBody] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound("No user with this email");

            // Generate a 6-digit code
            string code = new Random().Next(100000, 999999).ToString();
            var expires = DateTime.UtcNow.AddMinutes(10);
            await _resetService.DeleteByUserIdAsync(user.Id); // Remove old codes
            await _resetService.CreateAsync(new PasswordResetCode { UserId = user.Id, Code = code, ExpiresAt = expires });

            // Send code email
            string subject = "Your Cabin Booking Password Change Code";
            string body = $@"
                <div style='background:#f8fafc;padding:32px;font-family:sans-serif;'>
                  <div style='max-width:480px;margin:auto;background:white;border-radius:12px;box-shadow:0 2px 8px #0001;padding:32px;'>
                    <div style='text-align:center;margin-bottom:16px;'>
                      <img src='https://cdn-icons-png.flaticon.com/512/684/684908.png' alt='Cabin Booking' width='48' style='margin-bottom:8px;'>
                      <h2 style='color:#2563eb;margin:0;'>Password Change Code</h2>
                    </div>
                    <p style='font-size:1.1em;color:#444;'>Hi <b>{user.Name}</b>,</p>
                    <p style='font-size:1.1em;color:#444;'>Use the code below to change your Cabin Booking password:</p>
                    <div style='font-size:2em;letter-spacing:8px;background:#f1f5f9;padding:16px 0;margin:18px 0 24px 0;border-radius:8px;color:#2563eb;font-weight:bold;text-align:center;'>{code}</div>
                    <p style='color:#888;font-size:0.95em;'>This code is valid for 10 minutes.</p>
                    <p style='margin-top:32px;font-size:0.95em;color:#888;'>If you did not request this, you can ignore this email.</p>
                  </div>
                </div>
            ";
            await _emailService.SendEmailAsync(user.Email, subject, body);
            return Ok("Code sent");
        }

        [HttpPost("verify-password-code")]
        public async Task<IActionResult> VerifyPasswordCode([FromBody] PasswordCodeVerifyRequest req)
        {
            var user = await _userService.GetByEmailAsync(req.Email);
            if (user == null) return NotFound("No user with this email");
            var codeObj = await _resetService.GetByUserIdAsync(user.Id);
            if (codeObj == null || codeObj.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Code expired or not found");
            if (codeObj.Code != req.Code)
                return BadRequest("Invalid code");
            return Ok("Code valid");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest req)
        {
            var user = await _userService.GetByEmailAsync(req.Email);
            if (user == null) return NotFound("No user with this email");
            var codeObj = await _resetService.GetByUserIdAsync(user.Id);
            if (codeObj == null || codeObj.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Code expired or not found");
            if (codeObj.Code != req.Code)
                return BadRequest("Invalid code");
            user.Password = req.NewPassword;
            await _userService.UpdateAsync(user.Id, user);
            await _resetService.DeleteByUserIdAsync(user.Id);
            return Ok("Password changed successfully");
        }

    }

    public class PasswordCodeVerifyRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class PasswordChangeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
