using System;
using System.Collections.Generic;
using System.Linq;
using Membership.Services;
using RedditSharp;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.ReadModel;
using Post = RedditSharp.Things.Post;

namespace Skimur.TestData
{
    class Program
    {
        static readonly Reddit Reddit = new Reddit();

        static void Main(string[] args)
        {
            SkimurContext.ContainerInitialized += Skimur.Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Skimur.Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(new Skimur.Markdown.Registrar(),
                new Skimur.Scraper.Registrar(),
                new Subs.Registrar(),
                new Subs.Worker.Registrar(),
                new Membership.Registrar());

            using (SkimurContext.Resolve<IBusLifetime>())
            {
                var subDao = SkimurContext.Resolve<ISubDao>();
                var membershipService = SkimurContext.Resolve<IMembershipService>();
                var postDao = SkimurContext.Resolve<IPostDao>();
                var commentDao = SkimurContext.Resolve<ICommentDao>();

                var defaultUser = membershipService.GetUserByUserName("skimur");
                if (defaultUser == null)
                {
                    Console.WriteLine("You must have a user with the username 'skimur' created to insert test data.");
                    return;
                }
                
                foreach (var subToExamine in new List<string>
                {
                    "/r/news",
                    "/r/pics"
                })
                {
                    ExamineSubreddit(subToExamine,
                    subreddit =>
                    {
                        var sub = subDao.GetSubByName(subreddit.Name);

                        if (sub != null)
                            return sub;

                        var response = SkimurContext.Resolve<ICommandHandlerResponse<CreateSub, CreateSubResponse>>()
                            .Handle(new CreateSub
                            {
                                Name = subreddit.Name,
                                CreatedByUserId = defaultUser.Id,
                                Description = subreddit.Description,
                                SidebarText = subreddit.Description,
                                Type = Subs.SubType.Public
                            });

                        if (!string.IsNullOrEmpty(response.Error))
                        {
                            Console.WriteLine("Couldn't create the news sub. " + response.Error);
                            return null;
                        }

                        return subDao.GetSubByName(response.SubName);
                    },
                    (sub, redditPost) =>
                    {
                        var response = SkimurContext.Resolve<ICommandHandlerResponse<CreatePost, CreatePostResponse>>()
                           .Handle(new CreatePost
                           {
                               Url = redditPost.Url.OriginalString,
                               SubName = sub.Name,
                               PostType = redditPost.IsSelfPost ? Subs.PostType.Text : Subs.PostType.Link,
                               Title = redditPost.Title,
                               Content = redditPost.SelfText,
                               CreatedByUserId = defaultUser.Id,
                               IpAddress = "127.0.0.1",
                               NotifyReplies = true
                           });

                        if (!string.IsNullOrEmpty(response.Error))
                        {
                            Console.WriteLine("Error creating post. " + response.Error);
                            return null;
                        }

                        if (!response.PostId.HasValue)
                        {
                            Console.WriteLine("No post id returned.");
                            return null;
                        }

                        return postDao.GetPostById(response.PostId.Value);
                    },
                    (post, redditComment, parentComment) =>
                    {
                        var response = SkimurContext.Resolve<ICommandHandlerResponse<CreateComment, CreateCommentResponse>>()
                         .Handle(new CreateComment
                         {
                             PostId = post.Id,
                             ParentId = parentComment != null ? parentComment.Id : (Guid?)null,
                             DateCreated = redditComment.CreatedUTC,
                             AuthorUserName = defaultUser.UserName,
                             AuthorIpAddress = "127.0.0.1",
                             Body = redditComment.Body,
                             SendReplies = true
                         });

                        if (!string.IsNullOrEmpty(response.Error))
                        {
                            Console.WriteLine("Error creating comment. " + response.Error);
                            return null;
                        }

                        if (!response.CommentId.HasValue)
                        {
                            Console.WriteLine("No comment id returned.");
                            return null;
                        }

                        return commentDao.GetCommentById(response.CommentId.Value);
                    });
                }

                
            }
        }

        static void ExamineSubreddit(string name,
            Func<RedditSharp.Things.Subreddit, Subs.Sub> subredditFunc,
            Func<Subs.Sub, RedditSharp.Things.Post, Subs.Post> postFunc,
            Func<Subs.Post, RedditSharp.Things.Comment, Subs.Comment, Subs.Comment> commentFunc)
        {
            var redditSub = Reddit.GetSubreddit(name);

            var sub = subredditFunc(redditSub);
            if (sub == null) return;

            foreach (var redditPost in redditSub.Hot.Take(50))
            {
                var post = postFunc(sub, redditPost);
                if (post == null) continue;

                var comments = redditPost.ListComments(50);

                //var comments = redditPost.Comments.ToList();

                foreach (var comment in comments)
                {
                    RecursivelyInspectComments(commentFunc, post, comment);
                }
            }
        }

        static void RecursivelyInspectComments(Func<Subs.Post, RedditSharp.Things.Comment, Subs.Comment, Subs.Comment> commentFunc,
            Subs.Post post, 
            RedditSharp.Things.Comment redditComment, 
            Subs.Comment parentComment = null)
        {
            if (redditComment.Kind == "more")
            {
                return;
            }

            if (redditComment.Kind != "t1")
            {
                return;
            }

            var comment = commentFunc(post, redditComment, parentComment);
            if (comment == null) return;

            foreach (var childRedditComment in redditComment.Comments)
            {
                RecursivelyInspectComments(commentFunc, post, childRedditComment, comment);
            }
        }
    }
}
