using System;
using Skimur.Messaging.Handling;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker.Events
{
    public class KudosUpdateEventHandler :
        IEventHandler<VoteForPostCasted>,
        IEventHandler<VoteForCommentCasted>
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IKarmaService _karmaService;

        public KudosUpdateEventHandler(IPostService postService, 
            ICommentService commentService, 
            IKarmaService karmaService)
        {
            _postService = postService;
            _commentService = commentService;
            _karmaService = karmaService;
        }

        public void Handle(VoteForPostCasted @event)
        {
            var post = _postService.GetPostById(@event.PostId);
            if (post == null) return;

            var difference = ConvertVoteTypeToInteger(@event.VoteType) - ConvertVoteTypeToInteger(@event.PreviousVote);
            if (difference == 0) return;

            _karmaService.AdjustKarma(post.UserId, post.SubId, KarmaType.Post, difference);
        }

        public void Handle(VoteForCommentCasted @event)
        {
            var comment = _commentService.GetCommentById(@event.CommentId);
            if (comment == null) return;

            var difference = ConvertVoteTypeToInteger(@event.VoteType) - ConvertVoteTypeToInteger(@event.PreviousVote);
            if (difference == 0) return;

            _karmaService.AdjustKarma(comment.AuthorUserId, comment.SubId, KarmaType.Comment, difference);
        }

        private int ConvertVoteTypeToInteger(VoteType? vote)
        {
            switch (vote)
            {
                case null:
                    return 0;
                case VoteType.Up:
                    return 1;
                case VoteType.Down:
                    return -1;
                default:
                    throw new Exception("huh?");
            }
        }
    }
}
