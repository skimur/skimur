using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Subs.Commands;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class ReplyNotificationEventHandler : IEventHandler<CommentCreated>
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly ICommandBus _commandBus;

        public ReplyNotificationEventHandler(ICommentService commentService, 
            IPostService postService,
            ICommandBus commandBus)
        {
            _commentService = commentService;
            _postService = postService;
            _commandBus = commandBus;
        }

        public void Handle(CommentCreated @event)
        {
            var post = _postService.GetPostById(@event.PostId);
            if (post == null) return;
            
            var comment = _commentService.GetCommentById(@event.CommentId);
            if (comment == null) return;

            Comment replyToComment = null;
            if (comment.ParentId.HasValue)
            {
                replyToComment = _commentService.GetCommentById(comment.ParentId.Value);
                if (replyToComment == null) return;
            }

            if (replyToComment == null)
            {
                // the user replied to his own post. don't notify.
                if (post.UserId == comment.AuthorUserId) return;

                // this is a reply to a post
                if (post.SendReplies)
                {
                    // the user wan'ts to know about replies!
                    _commandBus.Send(new SendMessage
                    {
                        Author = comment.AuthorUserId,
                        AuthorIp = comment.AuthorIpAddress,
                        ToUserId = post.UserId,
                        Type = MessageType.PostReply,
                        CommentId = comment.Id
                    });
                }
            }
            else
            {
                // the user replied to his own comment. don't notify.
                if (replyToComment.AuthorUserId == comment.AuthorUserId) return;

                // this is a reply to a comment
                if (replyToComment.SendReplies)
                {
                    // the user wan't to know about replies!
                    _commandBus.Send(new SendMessage
                    {
                        Author = comment.AuthorUserId,
                        AuthorIp = comment.AuthorIpAddress,
                        ToUserId = replyToComment.AuthorUserId,
                        Type = MessageType.CommentReply,
                        CommentId = comment.Id
                    });
                }
            }
        }
    }
}
