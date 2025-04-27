using Backend.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class PasswordResetService
    {
        private readonly IMongoCollection<PasswordResetCode> _resetCodes;
        public PasswordResetService(MongoDbService mongoDbService)
        {
            _resetCodes = mongoDbService.GetCollection<PasswordResetCode>("password_reset_codes");
        }

        public async Task CreateAsync(PasswordResetCode code) =>
            await _resetCodes.InsertOneAsync(code);

        public async Task<PasswordResetCode> GetByUserIdAsync(string userId) =>
            await _resetCodes.Find(c => c.UserId == userId).FirstOrDefaultAsync();

        public async Task DeleteByUserIdAsync(string userId) =>
            await _resetCodes.DeleteManyAsync(c => c.UserId == userId);
    }
}
