using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MongoDB.Driver;
using DAB_Assignment_3.Models;

namespace DAB_Assignment_3.Services
{
    public class CommentServices
    {
        private IMongoCollection<Comment> _comments;
        private IMongoCollection<User> _users;
        private IMongoCollection<Circle> _circle;
        private IMongoCollection<Post> _post;

        public CommentServices(string connection)
        {
            var client = new MongoClient(connection);
            var database = client.GetDatabase("SocialNetworkDb");

            _comments = database.GetCollection<Comment>("Comments");
            _users = database.GetCollection<User>("Users");
            _post = database.GetCollection<Post>("Posts");
            _circle = database.GetCollection<Circle>("Circles");
        }

        public void CreateComment()
        {
            Console.Write("Input your UserID: ");
            string userId = Console.ReadLine();

            var user = _users.Find(x => x.Id == userId).FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("User does not exist");
                return;
            }

            Console.Write("Input Author of post's UserID: ");
            string AuthorUserId = Console.ReadLine();

            var auUser = _users.Find(x => x.Id == AuthorUserId).FirstOrDefault();
            if (auUser == null)
            {
                Console.WriteLine("Author does not exist");
                return;
            }
            else
            {
                if(auUser.UserPostsId.Count == 0)
                {
                    Console.WriteLine("Author has no posts");
                    return;
                }
                else
                {
                    foreach (var p in auUser.UserPostsId)
                    {
                        Console.WriteLine(p);
                    }
                }
                
            }


            Console.Write("Input PostID: ");
            string post_id = Console.ReadLine();

            //if no posts available
            var post = _post.Find(x => x.PostId == post_id ).FirstOrDefault();
            if (post == null)
            {
                Console.WriteLine("Invalid post");
                return;
            }
            else
            {
                if (post.IsPublic == true)
                { // if post is public
                    if (post.BlockedAllowedUserId.Contains(userId))
                    {
                        Console.WriteLine("UserID is blocked, you cannot write any comment.");
                    }
                    else
                    {
                        Console.Write("Write comment: ");
                        string c = Console.ReadLine();

                        var comment = new Comment(post_id, user.Id, user.Name, c,DateTime.Now);
                        Console.WriteLine("Comment added");
                        _comments.InsertOne(comment);
                    }
                }
                else
                { // if post is private.
                    if (post.BlockedAllowedUserId.Contains(userId))
                    {
                        Console.Write("Write comment: ");
                        string c = Console.ReadLine();

                        var comment = new Comment(post_id, user.Id, user.Name, c, DateTime.Now);
                        Console.WriteLine("Comment added");
                        _comments.InsertOne(comment);
                    }
                    else
                    {
                        Console.WriteLine("UserID is not a part of any of the user circles.");
                    }
                }
            }         

        }


    }
}
