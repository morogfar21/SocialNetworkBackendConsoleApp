using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAB_Assignment_3.Models
{
    public class User
    {
        public User(string name, int age, string gender)
        {
            FollowId = new List<string>();
            BlockId = new List<string>();
            UserPostsId = new List<string>();
            CircleId = new List<string>();
            CircleName = new List<string>();

            Name = name;
            Age = age;
            Gender = gender;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")] 
        public string Name { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Gender")] 
        public string Gender { get; set; }

        [BsonElement("FollowId")]
        public List<string> FollowId { get; set; }

        [BsonElement("BlockId")]
        public List<string> BlockId { get; set; }

        //Bruger vi ID til noget?
        [BsonElement("CircleId")]
        public List<string> CircleId { get; set; }

        [BsonElement("CircleName")]
        public List<string> CircleName { get; set; }

        [BsonElement("UserPostsId")]
        public List<string> UserPostsId { get; set; }
    }
}
