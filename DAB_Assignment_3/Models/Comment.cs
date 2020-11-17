using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace DAB_Assignment_3.Models
{
    public class Comment
    {
        public Comment(string postid, string authorid, string authorname, string commentstring, DateTime DateTime)
        {
            PostId = postid;
            AuthorId = authorid;
            AuthorName = authorname;
            CommentString = commentstring;
            this.DateTime = DateTime;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CommentId { get; set; }

        [BsonElement("PostId")]
        public string PostId { get; set; }

        [BsonElement("AuthorId")]
        public string AuthorId { get; set; }

        [BsonElement("AuthorName")]
        public string AuthorName { get; set; }

        [BsonElement("CommentString")]
        public string CommentString { get; set; }

        [BsonElement("DateTime")] 
        public DateTime DateTime { get; set; }
    }
}
