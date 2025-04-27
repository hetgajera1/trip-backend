using Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class ContactMessageService
    {
        private readonly IMongoCollection<ContactMessage> _contactMessages;

        public ContactMessageService(MongoDbService mongoDbService)
        {
            _contactMessages = mongoDbService.GetCollection<ContactMessage>("contactMessages");
        }

        public async Task<List<ContactMessage>> GetAllAsync() =>
            await _contactMessages.Find(_ => true).ToListAsync();

        public async Task<ContactMessage> GetByIdAsync(string id) =>
            await _contactMessages.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ContactMessage message) =>
            await _contactMessages.InsertOneAsync(message);
    }
}
