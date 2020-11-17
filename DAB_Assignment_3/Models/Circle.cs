using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Assignment_3.Models
{
    public class Circle
    {
        public Circle(string name, string ownerId)
        {
            UserIds = new List<string>();
            Name = name;
            OwnerId = ownerId;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CircleId { get; set; }

        [BsonElement("NameOfCircle")]
        public string Name { get; set; }

        [BsonElement("UserId")]
        public List<string> UserIds { get; set; }

        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }
    }
}
