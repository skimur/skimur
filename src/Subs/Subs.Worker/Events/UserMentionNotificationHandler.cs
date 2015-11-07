using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Services;
using Skimur.Messaging.Handling;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker.Events
{
    public class UserMentionNotificationHandler :
        IEventHandler<UsersMentioned>,
        IEventHandler<UsersUnmentioned>
    {
        private readonly ICommentService _commentService;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IMessageService _messageService;

        public UserMentionNotificationHandler(ICommentService commentService, 
            IMembershipService membershipService,
            IPostService postService,
            IMessageService messageService)
        {
            _commentService = commentService;
            _membershipService = membershipService;
            _postService = postService;
            _messageService = messageService;
        }

        public void Handle(UsersUnmentioned @event)
        {
            if (!@event.PostId.HasValue && !@event.CommentId.HasValue) return;

            if (@event.Users == null || @event.Users.Count == 0) return;

            var users = @event.Users.Select(x => _membershipService.GetUserByUserName(x)).Where(x => x != null).ToList();

            if (users.Count == 0) return;

            if (@event.CommentId.HasValue)
            {
                var comment = _commentService.GetCommentById(@event.CommentId.Value);
                if (comment == null) return;

                foreach (var user in users)
                {
                    _messageService.DeleteMention(user.Id, null, comment.Id);
                }
            }
            else
            {
                var post = _postService.GetPostById(@event.PostId.Value);
                if (post == null) return;

                foreach (var user in users)
                {
                    _messageService.DeleteMention(user.Id, post.Id, null);
                }
            }
        }

        public void Handle(UsersMentioned @event)
        {
            if (!@event.PostId.HasValue && !@event.CommentId.HasValue) return;

            if (@event.Users == null || @event.Users.Count == 0) return;

            var users = @event.Users.Select(x => _membershipService.GetUserByUserName(x)).Where(x => x != null).ToList();

            if (users.Count == 0) return;

            if (@event.CommentId.HasValue)
            {
                var comment = _commentService.GetCommentById(@event.CommentId.Value);
                if (comment == null) return;

                foreach (var user in users)
                {
                    _messageService.InsertMention(user.Id, comment.AuthorUserId, null, comment.Id);
                }
            }
            else
            {
                var post = _postService.GetPostById(@event.PostId.Value);
                if (post == null) return;

                foreach (var user in users)
                {
                    _messageService.InsertMention(user.Id, post.UserId, post.Id, null);
                }
            }
        }
    }
}
