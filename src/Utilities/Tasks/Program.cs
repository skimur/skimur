using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Messaging;
using Membership;
using Membership.Services;
using PowerArgs;
using ServiceStack.OrmLite;
using Skimur;
using Subs;
using Subs.Commands;
using Subs.Services;

namespace Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
            SkimurContext.ContainerInitialized += Infrastructure.Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Infrastructure.Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(new Infrastructure.Registrar(),
                new Infrastructure.Settings.Registrar(),
                new Infrastructure.Caching.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
                new Infrastructure.Cassandra.Registrar(),
                new Infrastructure.Postgres.Registrar(),
                new Infrastructure.Logging.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Subs.Registrar(),
                new Membership.Registrar());

            Args.InvokeAction<Tasks>(args);
        }
    }

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class Tasks
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Update kudos for all users")]
        public void UpdateKudos()
        {

            var membershipService = SkimurContext.Resolve<IMembershipService>();
            var karmaService = SkimurContext.Resolve<IKarmaService>();
            var postService = SkimurContext.Resolve<IPostService>();
            var commentService = SkimurContext.Resolve<ICommentService>();

            int currentIndex = 0;
            int pageSize = 10;

            var users = membershipService.GetAllUsers(currentIndex, pageSize);

            while (users.Count > 0)
            {
                foreach (var user in users)
                {
                    var postKudos = new Dictionary<Guid, int>();

                    foreach (var post in postService.GetPosts(userId: user.Id, hideRemovedPosts: false, showDeleted: true)
                        .Select(postId => postService.GetPostById(postId))
                        .Where(post => post != null))
                    {
                        if (!postKudos.ContainsKey(post.SubId))
                            postKudos.Add(post.SubId, 0);
                        postKudos[post.SubId] = postKudos[post.SubId] + (post.VoteUpCount - post.VoteDownCount);
                    }

                    var commentKudos = new Dictionary<Guid, int>();

                    foreach (var comment in commentService.GetCommentsForUser(user.Id)
                        .Select(commentId => commentService.GetCommentById(commentId))
                        .Where(comment => comment != null))
                    {
                        if (!commentKudos.ContainsKey(comment.SubId))
                            commentKudos.Add(comment.SubId, 0);
                        commentKudos[comment.SubId] = commentKudos[comment.SubId] + (comment.VoteUpCount - comment.VoteDownCount);
                    }

                    karmaService.DeleteAllKarmaForUser(user.Id);

                    if (postKudos.Count > 0)
                        foreach (var subId in postKudos.Keys)
                            karmaService.AdjustKarma(user.Id, subId, KarmaType.Post, postKudos[subId]);

                    if (commentKudos.Count > 0)
                        foreach (var subId in commentKudos.Keys)
                            karmaService.AdjustKarma(user.Id, subId, KarmaType.Comment, commentKudos[subId]);
                }

                currentIndex += users.Count;
                users = membershipService.GetAllUsers(currentIndex, pageSize);
            }
        }

        [ArgActionMethod, ArgDescription("Delete a sub")]
        public void DeleteSub(string subName)
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();

            var sub = connectionProvider.Perform(conn => conn.Single<Sub>(x => x.Name.ToLower() == subName.ToLower()));
            if (sub == null)
            {
                Console.WriteLine("No sub with that name.");
                return;
            }

            foreach (var post in connectionProvider.Perform(conn => conn.Select<Post>(x => x.SubId == sub.Id)))
            {
                DeletePost(post.Id);
            }

            foreach (var comment in connectionProvider.Perform(conn => conn.Select<Comment>(x => x.SubId == sub.Id)))
            {
                DeleteComment(comment.Id);
            }

            connectionProvider.Perform(conn => conn.Delete<Moderator>(x => x.SubId == sub.Id));
            connectionProvider.Perform(conn => conn.Delete<ModeratorInvite>(x => x.SubId == sub.Id));
            connectionProvider.Perform(conn => conn.Delete<SubCss>(x => x.SubId == sub.Id));
            connectionProvider.Perform(conn => conn.Delete<SubUserBan>(x => x.SubId == sub.Id));
            connectionProvider.Perform(conn => conn.Delete<Sub>(x => x.Id == sub.Id));
        }

        [ArgActionMethod, ArgDescription("Delete a user")]
        public void DeleteUser(string userName)
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();

            var user = connectionProvider.Perform(conn => conn.Single<User>(x => x.UserName.ToLower() == userName.ToLower()));
            if (user == null)
            {
                Console.WriteLine("No user with that name.");
                return;
            }

            // delete the users comments
            foreach (var comment in connectionProvider.Perform(conn => conn.Select<Comment>(x => x.AuthorUserId == user.Id)))
                DeleteComment(comment.Id);

            // delete the users posts
            foreach (var post in connectionProvider.Perform(conn => conn.Select<Post>(x => x.UserId == user.Id)))
                DeletePost(post.Id);
            
            connectionProvider.Perform(conn => conn.Delete<Message>(x => x.ToUser == user.Id));
            connectionProvider.Perform(conn => conn.Delete<Message>(x => x.AuthorId == user.Id));
        }
        
        [ArgActionMethod, ArgDescription("Delete comment")]
        public void DeleteComment(Guid commentId)
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();

            var comment = connectionProvider.Perform(conn => conn.Single<Comment>(x => x.Id == commentId));
            if (comment == null)
            {
                Console.WriteLine("No comment with that id.");
                return;
            }

            connectionProvider.Perform(conn => conn.Delete<Vote>(x => x.CommentId == comment.Id));
            connectionProvider.Perform(conn => conn.Delete<Report.CommentReport>(x => x.CommentId == comment.Id));
            
            // delete any messages associated with this comment
            foreach (var messageId in connectionProvider.Perform(conn => conn.Select<Message>(x => x.CommentId == comment.Id).Select(x => x.Id)))
                DeleteMessageThread(messageId);
            
            // delete child comments
            foreach (var childComment in connectionProvider.Perform(conn => conn.Select<Comment>(x => x.ParentId == comment.Id)))
                DeleteComment(childComment.Id);

            connectionProvider.Perform(conn => conn.Delete<Comment>(x => x.Id == comment.Id));
        }

        [ArgActionMethod, ArgDescription("Delete post")]
        public void DeletePost(Guid postId)
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();

            var post = connectionProvider.Perform(conn => conn.Single<Post>(x => x.Id == postId));
            if (post == null)
            {
                Console.WriteLine("No post with that id.");
                return;
            }

            connectionProvider.Perform(conn => conn.Delete<Vote>(x => x.PostId == post.Id));
            connectionProvider.Perform(conn => conn.Delete<Report.PostReport>(x => x.PostId == post.Id));
            
            // delete any messages associated with this post
            foreach (var messageId in connectionProvider.Perform(conn => conn.Select<Message>(x => x.PostId == post.Id).Select(x => x.Id)))
                DeleteMessageThread(messageId);
            connectionProvider.Perform(conn => conn.Delete<Post>(x => x.Id == post.Id));

            // delete all comments for this post
            foreach (var comment in connectionProvider.Perform(conn => conn.Select<Comment>(x => x.PostId == post.Id)))
                DeleteComment(comment.Id);

            connectionProvider.Perform(conn => conn.Delete<Post>(x => x.Id == post.Id));
        }

        [ArgActionMethod, ArgDescription("Delete message")]
        public void DeleteMessageThread(Guid messageId)
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();

            var message = connectionProvider.Perform(conn => conn.Single<Message>(x => x.Id == messageId));
            if (message == null)
            {
                Console.WriteLine("No message with give id.");
                return;
            }

            if (message.ParentId.HasValue)
                connectionProvider.Perform(conn => conn.Delete<Message>(x => x.ParentId == message.ParentId || x.Id == message.ParentId));

            connectionProvider.Perform(conn => conn.Delete<Message>(x => x.Id == messageId));
        }

        [ArgActionMethod, ArgDescription("Update all post thumbnails")]
        public void UpdateAllThumbnails()
        {
            var connectionProvider = SkimurContext.Resolve<IDbConnectionProvider>();
            var commandBus = SkimurContext.Resolve<ICommandBus>();

            var postIds = connectionProvider.Perform(
                conn => conn.Select(conn.From<Post>().Where(x => x.Type == (int)PostType.Link).Select(x => x.Id)).Select(x => x.Id));

            foreach (
                var postId in
                    postIds)
            {
                commandBus.Send(new GenerateThumbnailForPost
                {
                    PostId = postId
                });
            }
        }
    }

}
