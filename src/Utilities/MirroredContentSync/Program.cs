using System;
using System.Linq;
using MirroredContentSync.Settings;
using Skimur;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Skimur.Settings;
using Microsoft.Extensions.DependencyInjection;
using RedditSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Skimur.App;
using Skimur.App.Commands;
using Skimur.App.Services;

namespace MirroredContentSync
{
    public class Program : IRegistrar
    {
        private static MirrorSettings _mirrorSettings;
        private static ISubService _subService;
        private static IPostService _postService;
        private static IMembershipService _membershipService;
        private static ICommandBus _commandBus;



        static void Main(string[] args)
        {
            try
            {
                SkimurContext.Initialize(
                    new Program(),
                    new Skimur.App.Registrar());

                _mirrorSettings = SkimurContext.ServiceProvider.GetRequiredService<ISettingsProvider<MirrorSettings>>().Settings;
                _subService = SkimurContext.ServiceProvider.GetRequiredService<ISubService>();
                _postService = SkimurContext.ServiceProvider.GetRequiredService<IPostService>();
                _membershipService = SkimurContext.ServiceProvider.GetRequiredService<IMembershipService>();
                _commandBus = SkimurContext.ServiceProvider.GetRequiredService<ICommandBus>();

                if (_mirrorSettings.SubsToMirror == null || _mirrorSettings.SubsToMirror.Count == 0)
                    return;

                var botUser = _membershipService.GetUserByUserName(_mirrorSettings.BotName);
                if (botUser == null) return;

                var reddit = new Reddit();

                foreach (var subToMirror in _mirrorSettings.SubsToMirror)
                {
                    Console.WriteLine("Attempting to mirror " + subToMirror + ".");

                    var sub = _subService.GetSubByName(subToMirror);
                    if (sub == null)
                    {
                        Console.WriteLine("Sub doesn't exist.");
                        continue;
                    }

                    var redditSub = reddit.GetSubreddit("/r/" + subToMirror);
                    if (redditSub == null)
                    {
                        Console.WriteLine("Couldn't find reddit sub.");
                        continue;
                    }

                    foreach (var redditPost in redditSub.GetTop(_mirrorSettings.FromTime).Take(_mirrorSettings.PostsPerSub))
                    {
                        Console.WriteLine("Syncing " + redditPost.Title);

                        var existing = _postService.QueryPosts(redditPost.Title, sub.Id).Select(x => _postService.GetPostById(x)).ToList();
                        var exists = false;
                        if (existing.Count > 0)
                        {
                            foreach (var item in existing)
                            {
                                if (item.Title == redditPost.Title && item.Mirrored == "reddit")
                                    exists = true;
                            }
                        }
                        if (exists)
                        {
                            Console.WriteLine("Already exists.");
                            continue;
                        }

                        var createPostResponse = _commandBus.Send<CreatePost, CreatePostResponse>(
                            new CreatePost
                            {
                                CreatedByUserId = botUser.Id,
                                Title = redditPost.Title,
                                Url = redditPost.Url.ToString(),
                                Content = redditPost.SelfText,
                                PostType = redditPost.IsSelfPost ? PostType.Text : PostType.Link,
                                SubName = subToMirror,
                                NotifyReplies = false,
                                Mirror = "reddit",
                                OverrideDateCreated = redditPost.CreatedUTC
                            });

                        if (!string.IsNullOrEmpty(createPostResponse.Error))
                        {
                            Console.WriteLine("Couldn't create post. " + createPostResponse.Error);
                            continue;
                        }

                        if (!createPostResponse.PostId.HasValue)
                        {
                            Console.WriteLine("No post id");
                            continue;
                        }

                        var createCommentResponse = _commandBus.Send<CreateComment, CreateCommentResponse>(
                           new CreateComment
                           {
                               PostId = createPostResponse.PostId.Value,
                               DateCreated = Common.CurrentTime(),
                               AuthorUserName = botUser.UserName,
                               Body = string.Format("Mirrored from [here]({0}).", redditPost.Shortlink),
                               SendReplies = false
                           });

                        if (!string.IsNullOrEmpty(createCommentResponse.Error))
                        {
                            Console.WriteLine("Couldn't create comment. " + createCommentResponse.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int Order => 0;

        public void Register(IServiceCollection serviceCollection)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();

            serviceCollection.AddInstance<IConfiguration>(builder.Build());
        }
    }
}
