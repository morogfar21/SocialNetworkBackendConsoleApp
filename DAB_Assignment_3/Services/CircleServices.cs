using System;
using System.Collections.Generic;
using System.Text;
using DAB_Assignment_3.Models;
using MongoDB.Driver;

namespace DAB_Assignment_3.Services
{
    public class CircleServices
    {
        private IMongoCollection<User> _users;
        private IMongoCollection<Circle> _circles;

        public CircleServices(string connection)
        {
            var client = new MongoClient(connection);
            var database = client.GetDatabase("SocialNetworkDb");

            _users = database.GetCollection<User>("Users");
            _circles = database.GetCollection<Circle>("Circles");
        }

        public void CreateCircle(string ownerId, string circleName)
        {
            Console.WriteLine("Creating new circle");

            var c = new Circle(circleName,ownerId);

            _circles.InsertOne(c);
        }

        public void AddUser(string circleId, string userId)
        {
            var c = _circles.Find<Circle>(c => c.CircleId == circleId).FirstOrDefault();
            if (c == null)
            {
                Console.WriteLine("Invalid Id. Circle not found");
                return;
            }

            if (!c.UserIds.Contains(userId))
            {
                var updateUserId = Builders<Circle>.Update.AddToSet(circle => circle.UserIds, userId);
                _circles.FindOneAndUpdate(circle => circle.CircleId == circleId, updateUserId);
                //c.UserIds.Add(userId);
            }
                
            
            
            Console.WriteLine($"User with id {userId} added to {c.Name}");
        }

        public void RemoveUser(string circleId, string userId)
        {
            var c = _circles.Find<Circle>(c => c.CircleId == circleId).FirstOrDefault();

            if (c == null)
            {
                Console.WriteLine("Invalid Id. Circle not found");
                return;
            }

            if (c.UserIds.Contains(userId))
                c.UserIds.Remove(userId);

            Console.WriteLine($"User with id {userId} removed from {c.Name}");
        }

        public void DeleteCircle(string circleId)
        {
            var c = _circles.Find<Circle>(c => c.CircleId == circleId).FirstOrDefault();
            if (c == null)
            {
                Console.WriteLine("Invalid Id. Circle not found");
                return;
            }

            Console.WriteLine($"Circle {c.Name} will be permanently deleted, continue? (y/n)");
            var input = Console.ReadKey();

            if (input.ToString() != "y") return;

            _circles.DeleteOne(c => c.CircleId == circleId);
            Console.WriteLine("Circle deleted");
        }

    }
}
