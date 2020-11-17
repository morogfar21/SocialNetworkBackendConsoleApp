using System;
using System.Collections.Generic;
using System.Text;
using DAB_Assignment_3.Models;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace DAB_Assignment_3.Services
{
    public class UserServices
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _posts;
        private readonly IMongoCollection<Comment> _comments;

        public UserServices(string connection)
        {
            var client = new MongoClient(connection);
            var database = client.GetDatabase("SocialNetworkDb");

            _users = database.GetCollection<User>("Users");
            _posts = database.GetCollection<Post>("Posts");
            _comments = database.GetCollection<Comment>("Comments");
        }

        //Find all users
        public List<User> Get() =>
            _users.Find(user => true).ToList();

        //Create a user
        public void CreateUser()
        {
            Console.Write("Enter new username: ");
            string name = Console.ReadLine();
            Console.Write("\nEnter users age: ");
            int age = Convert.ToInt32(Console.ReadLine());
            Console.Write("\nEnter gender: ");
            string gender = Console.ReadLine();

            User user = new User(name, age, gender);

            _users.InsertOne(user);

            Console.WriteLine($"{gender}-user {name} (age: {age}) has been successfully created");
        }

        public void BlockUser(string userid, string blockUserId)
        {
            try
            {
                var updateFollowId = Builders<User>.Update.AddToSet(user => user.BlockId, blockUserId);
                _users.FindOneAndUpdate(user => user.Id == userid, updateFollowId);
            }
            catch (Exception)
            {
                Console.WriteLine("User doesn't exist");
                return;
            }
        }


        public void UnblockUser(string userid, string blockedUserId)
        {
            var userWhoBlocks = _users.Find(user => 
                    user.Id == userid &&
                    user.BlockId.Contains(blockedUserId)).FirstOrDefault();

            if (userWhoBlocks == null)
            {
                return;
            }

            userWhoBlocks.BlockId.Remove(blockedUserId);
        }

        public void Follow(string userid, string userToFollow)
        {

            try
            {
                var updateFollowId = Builders<User>.Update.AddToSet(user => user.FollowId, userToFollow);
                _users.FindOneAndUpdate(user => user.Id == userid, updateFollowId);
            }
            catch (Exception)
            {
                Console.WriteLine("User doesn't exist");
                return;
            }
        }

        public void UnFollow(string userid, string userToUnfollow)
        {
            var user = _users.Find(user =>
                user.Id == userid &&
                user.FollowId.Contains(userToUnfollow)).FirstOrDefault();

            if (user == null)
            {
                return;
            }

            user.FollowId.Remove(userToUnfollow);

            Console.WriteLine($"User {user.Name} has unfollowed user with ID: {userToUnfollow}");
        }

        //Find by UserId (Won't be good - dont know the Id's?)
        public User Get(string userid) =>
            _users.Find<User>(user => user.Id == userid).FirstOrDefault();

        //Get feed from er specific user
        public void GetFeed(string userid)
        {
            var user = _users.Find(findUser => findUser.Id == userid).FirstOrDefault();

            //Find user from whom, you want to see feed
            if (user == null)
            {
                Console.WriteLine("User doesn't exist");
                return;
            }


            var userFeed = _posts.Find(post =>
                    //Checking if post is private or public, if the use is blocked
                    //or not - and if the user follows the author of post
                    (post.IsPublic == false &&
                     post.BlockedAllowedUserId.Contains(userid) &&
                     user.FollowId.Contains(post.AuthorId)) ||

                    //If the post is public - we're checking if the user
                    //is in the "blockedAllowedUserId list (if the user is, post wont show in feed)
                    (post.IsPublic &&
                     !post.BlockedAllowedUserId.Contains(userid) &&
                     user.FollowId.Contains(post.AuthorId)
                    ))
                .SortByDescending(post => post.PostId).Limit(5).ToList();

            if (userFeed.Count == 0)
            {
                Console.WriteLine("User has no feed to see");
                return;
            }
            foreach (var f in userFeed)
            {
                if (f is DataPost)
                {
                    DataPost dp = (DataPost) f;
                    Console.WriteLine($"Feed: {dp.UrlToData}");
                }
                else if (f is TextPost)
                {
                    TextPost tp = (TextPost) f;
                    Console.WriteLine($"Feed: {tp.Text}");                }

                //Console.WriteLine($"Feed: {f}");

                var comments = _comments.Find(comment =>
                        comment.PostId == f.PostId)
                    .SortByDescending(comment => comment.PostId).Limit(5).ToList();

                if (comments == null)
                {
                    Console.WriteLine("There's no comment to the post on the feed");
                    return;
                }
                foreach (var c in comments)
                {
                    Console.WriteLine($"Comment: {c.CommentString} at {c.DateTime}");
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////
        }

        public void GetWall(string userid, string guestId)
        {
            var user = _users.Find(findUser => findUser.Id == userid).FirstOrDefault();
            var guest = _users.Find(findGuest => findGuest.Id == guestId).FirstOrDefault();

            if (user == null || guest == null)
            {
                Console.WriteLine("User or guest doesn't exist");
                return;
            }

            if (user.BlockId != null)
            {
                if (user.BlockId.Contains(guestId))
                {
                    Console.WriteLine("Guest is blocked by user!");
                    return;
                }
            }
            
            var wall = _posts.Find(postsOnWall =>
                postsOnWall.AuthorId == user.Id)
            .SortBy(post => post.DateTime).Limit(5).ToList();

            if (wall.Count == 0 || wall == null)
            {
                Console.WriteLine("There's no posts on the wall!");
                return;
            }

            foreach (var wp in wall)
            {
                if (!wp.IsPublic && !wp.BlockedAllowedUserId.Contains(guestId))
                {
                    continue;
                }

                if (wp is DataPost)
                {
                    DataPost dp = (DataPost)wp;
                    Console.WriteLine($"Wall: {dp.UrlToData}");
                }
                else if (wp is TextPost)
                {
                    TextPost tp = (TextPost)wp;
                    Console.WriteLine($"Wall: {tp.Text}");
                }

                var comments = _comments.Find(comment =>
                        comment.PostId == wp.PostId)
                    .SortByDescending(comment => comment.PostId).Limit(5).ToList();


                if (comments == null)
                {
                    Console.WriteLine("There's no comment to the post on the wall");
                    return;
                }

                foreach (var c in comments)
                {
                    Console.WriteLine($"Comment: {c.CommentString} at {c.DateTime}");
                }
            }
        }


    }
}
