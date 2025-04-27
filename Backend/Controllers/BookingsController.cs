using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Backend;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        // DEBUG: log all incoming POST booking payloads
        private void DebugLogBooking(Backend.Models.Booking booking)
        {
            Console.WriteLine("[DEBUG] Incoming booking object: " + Newtonsoft.Json.JsonConvert.SerializeObject(booking));
        }
        private readonly BookingService _bookingService;
        private readonly EmailService _emailService;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        public BookingsController(BookingService bookingService, UserService userService, IConfiguration configuration)
        {
            _bookingService = bookingService;
            _userService = userService;
            _configuration = configuration;
            _emailService = new EmailService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<Booking>>> Get() =>
            await _bookingService.GetAllAsync();

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Booking>>> GetByUser(string userId) =>
            await _bookingService.GetByUserIdAsync(userId);

        // NEW: Get bookings for the authenticated user
        [AllowAnonymous]
        [HttpGet("user")]
        public async Task<ActionResult<List<Booking>>> GetCurrentUserBookings([FromQuery]string userId = null)
        {
            Console.WriteLine($"[BookingsController] /user endpoint called. userId: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                // For testing, return ALL bookings if userId is not provided
                var allBookings = await _bookingService.GetAllAsync();
                return Ok(allBookings);
            }
            var bookings = await _bookingService.GetByUserIdAsync(userId);
            Console.WriteLine($"[BookingsController] Bookings fetched for userId {userId}: {Newtonsoft.Json.JsonConvert.SerializeObject(bookings)}");
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> Get(string id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return booking;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Booking booking)
        {
            DebugLogBooking(booking);
            await _bookingService.CreateAsync(booking);
            // Fetch user email
            var user = await _userService.GetByIdAsync(booking.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string subject = $"Booking Confirmed: {booking.CabinName} ({booking.CheckIn:dd MMM} - {booking.CheckOut:dd MMM yyyy})";
                string emailBody = $@"
                <div style='background:#f8fafc;padding:32px;font-family:sans-serif;'>
                  <div style='max-width:520px;margin:auto;background:white;border-radius:12px;box-shadow:0 2px 8px #0002;padding:32px;'>
                    <div style='text-align:center;margin-bottom:18px;'>
                      <img src='https://cdn-icons-png.flaticon.com/512/684/684908.png' alt='Cabin Booking' width='64' style='margin-bottom:8px;'>
                      <h1 style='color:#2563eb;margin:0;'>Booking Confirmed!</h1>
                    </div>
                    <p style='font-size:1.1em;color:#444;'>Hi <b>{user.Name}</b>,</p>
                    <p style='font-size:1.1em;color:#444;'>Your booking for <b>{booking.CabinName}</b> is confirmed 🎉</p>
                    <table style='width:100%;margin:18px 0 24px 0;border-collapse:collapse;'>
                      <tr><td style='color:#888;padding:4px 0;'>Check-in:</td><td style='color:#222;padding:4px 0;'>{booking.CheckIn:dddd, dd MMM yyyy}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Check-out:</td><td style='color:#222;padding:4px 0;'>{booking.CheckOut:dddd, dd MMM yyyy}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Guests:</td><td style='color:#222;padding:4px 0;'>{booking.NumberOfGuests}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Booking ID:</td><td style='color:#222;padding:4px 0;'>{booking.Id}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Total:</td><td style='color:#222;padding:4px 0;'>₹{booking.TotalAmount ?? 0:0.00}</td></tr>
                    </table>
                    <div style='margin:20px 0;'>
                      <a href='https://your-cabin-booking-site.com/bookings' style='display:inline-block;padding:12px 28px;background:#2563eb;color:white;text-decoration:none;border-radius:6px;font-weight:bold;font-size:1.1em;'>View My Booking</a>
                    </div>
                    <h3 style='color:#2563eb;margin-top:32px;'>Booking Details</h3>
                    <ul style='color:#2d3748;font-size:1em;margin:16px 0 24px 20px;'>
                      {(booking.EarlyCheckIn ? "<li>🕒 Early Check-in</li>" : "")}
                      {(booking.LateCheckout ? "<li>🌙 Late Checkout</li>" : "")}
                      {(booking.BreakfastPackage ? "<li>🍳 Breakfast Package</li>" : "")}
                      {(booking.WinePackage ? "<li>🍷 Wine Package</li>" : "")}
                      {(booking.FirewoodPackage ? "<li>🔥 Firewood Package</li>" : "")}
                      {(booking.TourGuide ? "<li>🗺️ Tour Guide</li>" : "")}
                    </ul>
                    {(string.IsNullOrEmpty(booking.SpecialRequests) ? "" : $"<p style='color:#444;'><b>Special Requests:</b> {booking.SpecialRequests}</p>")}
                    <p style='margin-top:32px;font-size:0.95em;color:#888;'>If you have any questions, reply to this email or contact our support team.<br><br>We wish you a wonderful stay!<br>— The Cabin Booking Team</p>
                  </div>
                </div>
                ";
                await _emailService.SendEmailAsync(user.Email, subject, emailBody);
            }
            return CreatedAtAction(nameof(Get), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, Booking booking)
        {
            var exist = await _bookingService.GetByIdAsync(id);
            if (exist == null) return NotFound();
            await _bookingService.UpdateAsync(id, booking);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            // Fetch user email before deleting
            var user = booking.UserId != null ? await _userService.GetByIdAsync(booking.UserId) : null;
            await _bookingService.RemoveAsync(id);
            // Send cancellation email
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string subject = $"Booking Cancelled: {booking.CabinName} ({booking.CheckIn:dd MMM} - {booking.CheckOut:dd MMM yyyy})";
                string emailBody = $@"
                <div style='background:#fff7f7;padding:32px;font-family:sans-serif;'>
                  <div style='max-width:520px;margin:auto;background:white;border-radius:12px;box-shadow:0 2px 8px #0002;padding:32px;'>
                    <div style='text-align:center;margin-bottom:18px;'>
                      <img src='https://cdn-icons-png.flaticon.com/512/684/684908.png' alt='Cabin Booking' width='64' style='margin-bottom:8px;opacity:0.7;'>
                      <h1 style='color:#e53e3e;margin:0;'>Booking Cancelled</h1>
                    </div>
                    <p style='font-size:1.1em;color:#444;'>Hi <b>{user.Name}</b>,</p>
                    <p style='font-size:1.1em;color:#444;'>Your booking for <b>{booking.CabinName}</b> has been <span style='color:#e53e3e;font-weight:bold;'>cancelled</span>.</p>
                    <table style='width:100%;margin:18px 0 24px 0;border-collapse:collapse;'>
                      <tr><td style='color:#888;padding:4px 0;'>Check-in:</td><td style='color:#222;padding:4px 0;'>{booking.CheckIn:dddd, dd MMM yyyy}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Check-out:</td><td style='color:#222;padding:4px 0;'>{booking.CheckOut:dddd, dd MMM yyyy}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Guests:</td><td style='color:#222;padding:4px 0;'>{booking.NumberOfGuests}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Booking ID:</td><td style='color:#222;padding:4px 0;'>{booking.Id}</td></tr>
                      <tr><td style='color:#888;padding:4px 0;'>Total:</td><td style='color:#222;padding:4px 0;'>₹{booking.TotalAmount ?? 0:0.00}</td></tr>
                    </table>
                    <div style='margin:20px 0;'>
                      <a href='https://your-cabin-booking-site.com/bookings' style='display:inline-block;padding:12px 28px;background:#e53e3e;color:white;text-decoration:none;border-radius:6px;font-weight:bold;font-size:1.1em;'>Book Again</a>
                    </div>
                    <p style='margin-top:32px;font-size:1em;color:#888;'>We're sorry to see your booking cancelled. If this was a mistake or you have questions, please reply to this email or contact our support team.<br><br>We hope to host you soon!<br>— The Cabin Booking Team</p>
                  </div>
                </div>
                ";
                await _emailService.SendEmailAsync(user.Email, subject, emailBody);
            }
            return NoContent();
        }
    }
}
