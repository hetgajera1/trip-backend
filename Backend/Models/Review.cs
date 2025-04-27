using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Backend.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CabinId { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
