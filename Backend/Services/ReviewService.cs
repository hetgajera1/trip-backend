using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class ReviewService
    {
        private readonly IMongoCollection<Review> _reviews;
        public ReviewService(MongoDbService mongoDbService)
        {
            _reviews = mongoDbService.GetCollection<Review>("reviews");
        }

        public async Task<List<Review>> GetByCabinIdAsync(string cabinId) =>
            await _reviews.Find(r => r.CabinId == cabinId).ToListAsync();

        public async Task<Review> GetByIdAsync(string id) =>
            await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Review review) =>
            await _reviews.InsertOneAsync(review);
    }
}
