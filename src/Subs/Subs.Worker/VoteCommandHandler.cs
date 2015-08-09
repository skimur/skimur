using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Membership;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Subs.Commands;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class VoteCommandHandler : 
        ICommandHandler<CastVoteForPost>,
        ICommandHandler<CastVoteForComment>
    {
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly ICommentService _commentService;
        private readonly IEventBus _eventBus;

        public VoteCommandHandler(IMembershipService membershipService,
            IPostService postService,
            IVoteService voteService,
            ICommentService commentService,
            IEventBus eventBus)
        {
            _membershipService = membershipService;
            _postService = postService;
            _voteService = voteService;
            _commentService = commentService;
            _eventBus = eventBus;
        }

        public void Handle(CastVoteForPost command)
        {
            var user = _membershipService.GetUserById(command.UserId);

            if (user == null)
                return;

            var post = _postService.GetPostById(command.PostId);

            if (post == null)
                return;

            if (command.VoteType.HasValue)
                _voteService.VoteForPost(post.Id, user.Id, command.IpAddress, command.VoteType.Value, command.DateCasted);
            else
                _voteService.UnVotePost(post.Id, user.Id);

            _eventBus.Publish(new VoteForPostCasted { PostId = post.Id, UserId = user.Id, VoteType = command.VoteType });
        }

        public void Handle(CastVoteForComment command)
        {
            var user = _membershipService.GetUserByUserName(command.UserName);

            if (user == null)
                return;

            var comment = _commentService.GetCommentById(command.CommentId);

            if (comment == null)
                return;

            if (command.VoteType.HasValue)
                _voteService.VoteForComment(comment.Id, user.Id, command.IpAddress, command.VoteType.Value, command.DateCasted);
            else
                _voteService.UnVoteComment(comment.Id, user.Id);

            _eventBus.Publish(new VoteForCommentCasted { CommentId = comment.Id, UserName = user.UserName, VoteType = command.VoteType });
        }
    }
}
