using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Birthday { get; set; }
        public string? ProfileImage { get; set; }
        public Preferences? Preferences { get; set; }
        public string? MemberSince { get; set; }
    }

    public class Preferences
    {
        public bool? Notifications { get; set; }
        public bool? Newsletter { get; set; }
        public bool? DealAlerts { get; set; }
    }
}
