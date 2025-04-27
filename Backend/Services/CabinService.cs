using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class CabinService
    {
        private readonly IMongoCollection<Cabin> _cabins;
        public CabinService(MongoDbService mongoDbService)
        {
            _cabins = mongoDbService.GetCollection<Cabin>("cabins");
        }

        public async Task<List<Cabin>> GetAllAsync() =>
            await _cabins.Find(_ => true).ToListAsync();

        public async Task<Cabin> GetByIdAsync(string id) =>
            await _cabins.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Cabin cabin) =>
            await _cabins.InsertOneAsync(cabin);

        public async Task UpdateAsync(string id, Cabin cabinIn) =>
            await _cabins.ReplaceOneAsync(c => c.Id == id, cabinIn);

        public async Task DeleteAsync(string id) =>
            await _cabins.DeleteOneAsync(c => c.Id == id);
    }
}
