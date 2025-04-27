using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Backend.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDbSettings:ConnectionString"];
            var databaseName = configuration["MongoDbSettings:DatabaseName"];
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
