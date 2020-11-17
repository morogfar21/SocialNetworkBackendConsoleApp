using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Assignment_3.Models
{
    public abstract class Post
    {
        public Post()
        {
            BlockedAllowedUserId = new List<string>();
            DateTime = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        [BsonElement("AuthorName")]
        public string AuthorName { get; set; }

        [BsonElement("AuthorId")]
        public string AuthorId { get; set; }

        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }

        [BsonElement("IsPublic")]
        public bool IsPublic { get; set; }


        //If IsPublic == true
        // -- > blockedList
        //If IsPublic == false
        // -- > AllowedList
        [BsonElement("BlockedAllowedUserId")]
        public List<string> BlockedAllowedUserId { get; set; }
    }
}
