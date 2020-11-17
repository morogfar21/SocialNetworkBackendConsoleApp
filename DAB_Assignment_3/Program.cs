using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAB_Assignment_3.Models;
using DAB_Assignment_3.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAB_Assignment_3
{
    

    class Program
    {
        //Connection string - TILPAS "localhost:27017" til egen PC.
        static string connectionString = "mongodb://localhost:27017";

        static MongoClient client = new MongoClient(connectionString);
        
        static void Main(string[] args)
        {
            //Remove database
            client.DropDatabase("SocialNetworkDb");

            //Creates database if not exist
            var database = GetDatabase("SocialNetworkDb");
            CreateSocialNetworkCollections(database);

            var circleServices = new CircleServices(connectionString);
            var commentServices = new CommentServices(connectionString);
            var postServices = new PostServices(connectionString);
            var userServices = new UserServices(connectionString);

            var _users = database.GetCollection<User>("Users");
            var dd = new DummyData();

            dd.InsertDummyData(database, connectionString);
            while (true)
            {
                DisplayMainChoices();
                var input = UserInput("Input Selection");

                switch (input)
                {
                    case "1":
                        ListAllUsers(userServices);
                        break;
                    case "2":
                        UserMenu(userServices);
                        break;
                    case "3":
                        postServices.CreatePost();
                        break;
                    case "4":
                        commentServices.CreateComment();
                        break;
                    case "5":
                        userServices.CreateUser();
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }

        private static void UserMenu(UserServices userServices)
        {
            List<User> users = userServices.Get();

            while (true)
            {
                var userId = UserInput("Input user id or 'esc' to go back");
                if (userId == "esc")
                    return;

                bool idExists = users.Any(u => u.Id == userId);

                if (!idExists)
                    Console.WriteLine("Invalid input try again");
                else
                {
                    Console.WriteLine("Choices:");
                    Console.WriteLine("1: Get user feed");
                    Console.WriteLine("2: Get Wall as guest");
                    var selection = UserInput("Input Selection");
                    switch (selection)
                    {
                        case "1":
                            userServices.GetFeed(userId);
                            break;
                        case "2":
                            userServices.GetWall(userId,UserInput("Input guest id:"));
                            break;
                        default:
                            Console.WriteLine("Wrong selection try again");
                            break;
                    }
                }
            }
        }

        private static void ListAllUsers(UserServices userServices)
        {
            List<User> users = userServices.Get();
            foreach (var user in users)
            {
                Console.WriteLine($"Name: {user.Name} Id: {user.Id}");
            }
        }

        private static void SelectUser(string userId)
        {
            
        }

        private static void DisplayMainChoices()
        {
            Console.WriteLine("Here are your choices");
            Console.WriteLine("1: List all Users");
            Console.WriteLine("2: Select user by id");
            Console.WriteLine("3: Create Post");
            Console.WriteLine("4: Create Comment");
            Console.WriteLine("5: Create user");
        }

        private static string UserInput(string outputToUser)
        {
            Console.WriteLine(outputToUser);
            return Console.ReadLine();
        }

        //Not working
        public static void ClearDatabase(string db)
        {
            client.DropDatabase("db");
        }
        //========

        public static IMongoDatabase GetDatabase(string name)
        {
            var database = client.GetDatabase(name);
            return (database);
        }

        public static void CreateSocialNetworkCollections(IMongoDatabase database)
        {
            database.CreateCollection("Circles");
            database.CreateCollection("Comments");
            database.CreateCollection("Posts");
            database.CreateCollection("Users");
        }

    }

}
