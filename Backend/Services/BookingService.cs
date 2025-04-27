using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class BookingService
    {
        private readonly IMongoCollection<Booking> _bookings;
        public BookingService(MongoDbService mongoDbService)
        {
            _bookings = mongoDbService.GetCollection<Booking>("bookings");
        }

        public async Task<List<Booking>> GetAllAsync() =>
            await _bookings.Find(_ => true).ToListAsync();

        public async Task<List<Booking>> GetByUserIdAsync(string userId) =>
            await _bookings.Find(b => b.UserId == userId).ToListAsync();

        public async Task<Booking> GetByIdAsync(string id) =>
            await _bookings.Find(b => b.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Booking booking) =>
            await _bookings.InsertOneAsync(booking);

        public async Task UpdateAsync(string id, Booking bookingIn) =>
            await _bookings.ReplaceOneAsync(b => b.Id == id, bookingIn);

        public async Task RemoveAsync(string id) =>
            await _bookings.DeleteOneAsync(b => b.Id == id);
    }
}
