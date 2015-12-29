using Skimur.App.Commands;
using Skimur.App.Events;
using Skimur.App.Services;
using Skimur.Messaging;
using Skimur.Messaging.Handling;

namespace Skimur.App.Handlers.Commands
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
        private readonly ISubUserBanService _subUserBanService;

        public VoteCommandHandler(IMembershipService membershipService,
            IPostService postService,
            IVoteService voteService,
            ICommentService commentService,
            IEventBus eventBus,
            ISubUserBanService subUserBanService)
        {
            _membershipService = membershipService;
            _postService = postService;
            _voteService = voteService;
            _commentService = commentService;
            _eventBus = eventBus;
            _subUserBanService = subUserBanService;
        }

        public void Handle(CastVoteForPost command)
        {
            var user = _membershipService.GetUserById(command.UserId);

            if (user == null)
                return;

            var post = _postService.GetPostById(command.PostId);

            if (post == null)
                return;

            if (post.Deleted)
            {
                // if the post is deleted, any user other than the author can't cast a vote.
                if (user.Id != post.UserId) return;

                // also, that vote that the author may cast can only be a unvote (remove vote).
                if (command.VoteType != null) return;
            }

            if (!user.IsAdmin)
                if (_subUserBanService.IsUserBannedFromSub(post.SubId, user.Id))
                    return;
            
            var currentVote = _voteService.GetVoteForUserOnPost(user.Id, post.Id);

            if (currentVote == command.VoteType) return; // already voted with that type!

            if (command.VoteType.HasValue)
                _voteService.VoteForPost(post.Id, user.Id, command.IpAddress, command.VoteType.Value, command.DateCasted);
            else
                _voteService.UnVotePost(post.Id, user.Id);

            _eventBus.Publish(new VoteForPostCasted { PostId = post.Id, UserId = user.Id, PreviousVote = currentVote, VoteType = command.VoteType });
        }

        public void Handle(CastVoteForComment command)
        {
            var user = _membershipService.GetUserById(command.UserId);

            if (user == null)
                return;

            var comment = _commentService.GetCommentById(command.CommentId);

            if (comment == null)
                return;

            if (comment.Deleted)
            {
                // if the comment is deleted, any user other than the author can't cast a vote.
                if (user.Id != comment.AuthorUserId) return;

                // also, that vote that the author may cast can only be a unvote (remove vote).
                if (command.VoteType != null) return;
            }

            var post = _postService.GetPostById(comment.PostId);

            if (post == null)
                return;

            if (!user.IsAdmin)
                if (_subUserBanService.IsUserBannedFromSub(post.SubId, user.Id))
                    return;

            var currentVote = _voteService.GetVoteForUserOnComment(user.Id, comment.Id);

            if (currentVote == command.VoteType) return; // already voted with that type!

            if (command.VoteType.HasValue)
                _voteService.VoteForComment(comment.Id, user.Id, command.IpAddress, command.VoteType.Value, command.DateCasted);
            else
                _voteService.UnVoteComment(comment.Id, user.Id);

            _eventBus.Publish(new VoteForCommentCasted { CommentId = comment.Id, UserName = user.UserName, PreviousVote = currentVote, VoteType = command.VoteType });
        }
    }
}
