using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using DAB_Assignment_3.Models;
using Microsoft.VisualBasic;
using MongoDB.Driver;

namespace DAB_Assignment_3.Services
{
    public class DummyData
    {
        public void InsertDummyData(IMongoDatabase database, string connection)
        {
            var users = database.GetCollection<User>("Users");
            var posts = database.GetCollection<Post>("Posts");
            var comments = database.GetCollection<Comment>("Comments");
            var circles = database.GetCollection<Circle>("Circles");
            var userService = new UserServices(connection);

            var rand = new Random(DateTime.Now.Millisecond);
            var usersList = new List<User>();

            #region Users

            for (int i = 0; i < 20; i++)
            {
                var r = rand.Next(2);
                var gender = r == 1 ? "M" : "F";
                var user = new User("Name_" + i.ToString(), i + 10, gender);
                usersList.Add(user);
            }
            users.InsertMany(usersList);

            usersList = users.Find(u => true).ToList();

            foreach (var u in usersList)
            {
                foreach (var un in usersList.Where(un => un != u))
                {
                    var uUpdate = users.Find<User>(us => us.Id == u.Id).FirstOrDefault();
                    if (rand.Next(3) == 2)
                    {
                        userService.BlockUser(u.Id,un.Id);
                    }
                    else
                    {
                        userService.Follow(u.Id,un.Id);
                    }
                }
            }

            #endregion

            #region Circle
            
            foreach (var u in usersList)
            {
                var c1 = new Circle(u.Name+"_C1", u.Id);
                var c2 = new Circle(u.Name + "_C2", u.Id);
                var c3 = new Circle(u.Name + "_C3", u.Id);
                foreach (var un in usersList.Where(un => un != u))
                {
                    var random = rand.Next(3);
                    if (random == 0)
                        c1.UserIds.Add(un.Id);
                    else if (random == 1)
                        c2.UserIds.Add(un.Id);
                    else if (random == 2) c3.UserIds.Add(un.Id);
                }
                circles.InsertOne(c1);
                circles.InsertOne(c2);
                circles.InsertOne(c3);

                var c1d = circles.Find<Circle>(c => c.Name == u.Name + "_C1").FirstOrDefault();
                var c2d = circles.Find<Circle>(c => c.Name == u.Name + "_C2").FirstOrDefault();
                var c3d = circles.Find<Circle>(c => c.Name == u.Name + "_C3").FirstOrDefault();

                var updateCircle1Id = Builders<User>.Update.AddToSet(u => u.CircleId, c1d.CircleId);
                var updateCircle2Id = Builders<User>.Update.AddToSet(u => u.CircleId, c2d.CircleId);
                var updateCircle3Id = Builders<User>.Update.AddToSet(u => u.CircleId, c3d.CircleId);

                users.FindOneAndUpdate(us => us.Id == u.Id, updateCircle1Id);
                users.FindOneAndUpdate(us => us.Id == u.Id, updateCircle2Id);
                users.FindOneAndUpdate(us => us.Id == u.Id, updateCircle3Id);

            }
            usersList = users.Find(u => true).ToList();
            #endregion


            #region Post
            int timeToAdd = 0;
            //Dummy Posts
            foreach (var user in usersList)
            {
                TextPost textPost = new TextPost();
                DataPost dataPost = new DataPost();

                textPost.AuthorId = user.Id;
                dataPost.AuthorId = user.Id;

                textPost.AuthorName = user.Name;
                dataPost.AuthorName = user.Name;

                bool randPublicPost = rand.Next(2) == 1 ? true : false;

                textPost.IsPublic = randPublicPost;
                dataPost.IsPublic = randPublicPost;

                string publicPost = randPublicPost == true ? "public" : "private";

                textPost.Text = $"My name is {textPost.AuthorName} and my ID is {textPost.AuthorId}. This is a {publicPost} TEXTPOST.";
                dataPost.UrlToData = $"Audio: 'My name is {dataPost.AuthorName} and my ID is {dataPost.AuthorId}. This is a {publicPost} DATAPOST.'";

                //Overwrite DateTime
                textPost.DateTime.AddHours(timeToAdd);
                dataPost.DateTime.AddHours(timeToAdd);
                timeToAdd++;

                posts.InsertOne(textPost);
                posts.InsertOne(dataPost);

                if (randPublicPost == true)
                {
                    foreach (var id in user.BlockId)
                    {
                        var updateBlockedAllowedUserId = Builders<Post>.Update.AddToSet(post => post.BlockedAllowedUserId, id);
                        posts.FindOneAndUpdate(post => post.AuthorId == user.Id, updateBlockedAllowedUserId);

                    }
                }
                else
                {
                    for (int i = 0; i < user.CircleId.Count; i++)
                    {
                        var c = circles.Find<Circle>(c => c.CircleId == user.CircleId[i]).FirstOrDefault();

                        var updateBlockedAllowedUserId = Builders<Post>.Update.AddToSet(post => post.BlockedAllowedUserId, c.CircleId);
                        posts.FindOneAndUpdate(post => post.AuthorId == user.Id, updateBlockedAllowedUserId);
                    }
                }

                try
                {
                    var updateUserPostId = Builders<User>.Update.AddToSet(user => user.UserPostsId, textPost.PostId);
                    users.FindOneAndUpdate(user => user.Id == textPost.AuthorId, updateUserPostId);
                }
                catch (Exception)
                {
                    Console.WriteLine("User doesn't exist");
                    return;
                }

                try
                {
                    var updateUserPostId = Builders<User>.Update.AddToSet(user => user.UserPostsId, dataPost.PostId);
                    users.FindOneAndUpdate(user => user.Id == dataPost.AuthorId, updateUserPostId);
                }
                catch (Exception)
                {
                    Console.WriteLine("User doesn't exist");
                    return;
                }


                

                //Insert comments to post
                InsertCommentDummyData(database,textPost,dataPost);
            }

            #endregion

            
        }

        public static void InsertCommentDummyData(IMongoDatabase database, TextPost tp, DataPost dp)
        {
            var comments = database.GetCollection<Comment>("Comments");

            var comment1 = new Comment(tp.PostId, tp.AuthorId, tp.AuthorName, commentstring: "Hvor er du?", new DateTime(year: 2019, month: 11, day: 29, hour: 19, minute: 3, second: 43));
            var comment2 = new Comment(dp.PostId, dp.AuthorId, dp.AuthorName, commentstring: "HVOR ER DU?", new DateTime(year: 2019, month: 11, day: 29, hour: 19, minute: 3, second: 43));
            var comment3 = new Comment(tp.PostId, tp.AuthorId, tp.AuthorName, commentstring: "WebAPI load", new DateTime(year: 2019, month: 11, day: 29, hour: 11, minute: 23, second: 23));
            var comment4 = new Comment(dp.PostId, dp.AuthorId, dp.AuthorName, commentstring: "MongoDb virker ikke", new DateTime(year: 2019, month: 11, day: 29, hour: 11, minute: 23, second: 28));
            var comment5 = new Comment(tp.PostId, tp.AuthorId, tp.AuthorName, commentstring: "Nu virker det!", new DateTime(year: 2019, month: 11, day: 29, hour: 11, minute: 53, second: 16));
            var comment6 = new Comment(dp.PostId, dp.AuthorId, dp.AuthorName, commentstring: "Giver det mening??", new DateTime(year: 2019, month: 11, day: 29, hour: 7, minute: 23, second: 23));
            var comment7 = new Comment(tp.PostId, tp.AuthorId, tp.AuthorName, commentstring: "Nu er jeg vaagen", new DateTime(year: 2019, month: 11, day: 29, hour: 10, minute: 23, second: 23));
            var comment8 = new Comment(dp.PostId, dp.AuthorId, dp.AuthorName, commentstring: "Du kan bare komme forbi kontoret", new DateTime(year: 2019, month: 11, day: 30, hour: 8, minute: 23, second: 23));
            var comment9 = new Comment(tp.PostId, tp.AuthorId, tp.AuthorName, commentstring: "Jeg tager papir med", new DateTime(year: 2019, month: 11, day: 18, hour: 4, minute: 40, second: 33));
            var comment10 = new Comment(dp.PostId, dp.AuthorId, dp.AuthorName, commentstring: "Kravsspec og review", new DateTime(year: 2019, month: 11, day: 29, hour: 10, minute: 20, second: 0));
        
            comments.InsertOne(comment1);
            comments.InsertOne(comment2);
            comments.InsertOne(comment3);
            comments.InsertOne(comment4);
            comments.InsertOne(comment5);
            comments.InsertOne(comment6);
            comments.InsertOne(comment7);
            comments.InsertOne(comment8);
            comments.InsertOne(comment9);
            comments.InsertOne(comment10);
        }

    }
}
