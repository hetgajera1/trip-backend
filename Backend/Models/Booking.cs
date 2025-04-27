using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Backend.Models
{
    public class Booking
    {
        // Existing fields
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? CabinId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string? Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string? ConfirmationCode { get; set; }
        // Cabin name
        public string? CabinName { get; set; }
        // Additional booking details
        public int NumberOfGuests { get; set; }
        public bool EarlyCheckIn { get; set; }
        public bool LateCheckout { get; set; }
        public bool FirewoodPackage { get; set; }
        public bool BreakfastPackage { get; set; }
        public bool WinePackage { get; set; }
        public bool TourGuide { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? SpecialRequests { get; set; }
        // Total amount for booking
        public decimal? TotalAmount { get; set; }
    }
}
