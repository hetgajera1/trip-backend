using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Cabin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Sleeps { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> Images { get; set; }
    }
}
