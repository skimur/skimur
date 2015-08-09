using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class ScoringAndSortingEventHandler : 
        IEventHandler<VoteForPostCasted>,
        IEventHandler<VoteForCommentCasted>
    {
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly ICommentService _commentService;

        public ScoringAndSortingEventHandler(IPostService postService, IVoteService voteService, ICommentService commentService)
        {
            _postService = postService;
            _voteService = voteService;
            _commentService = commentService;
        }

        public void Handle(VoteForPostCasted @event)
        {
            // update the total number of up/down votes for the post
            int upVotes;
            int downVotes;
            _voteService.GetTotalVotesForPost(@event.PostId, out upVotes, out downVotes);
            _postService.UpdatePostVotes(@event.PostId, upVotes, downVotes);
        }

        public void Handle(VoteForCommentCasted @event)
        {
            // update for sort values for this comment
            UpdateSortsForComment(@event.CommentId);
        }

        private void UpdateSortsForComment(Guid commentId)
        {
            var comment = _commentService.GetCommentById(commentId);
            if (comment == null) return;

            var post = _postService.GetPostById(comment.PostId);
            if (post == null) return;

            int upVotes;
            int downVotes;
            _voteService.GetTotalVotesForComment(comment.Id, out upVotes, out downVotes);
            _commentService.UpdateCommentVotes(comment.Id, upVotes, downVotes);

            var opChildren = _commentService.GetChildrenForComment(comment.Id, comment.AuthorUserId);

            // general/best sort for comments
            var confidence = Sorting.Confidence(upVotes, downVotes);

            // get the general/best sort, with a modifier to adjust score for long comments.
            // also, if this comment has a reply from op, then the question will be boosted.
            var qa = Sorting.Qa(upVotes, downVotes, string.IsNullOrEmpty(comment.Body) ? 0 : comment.Body.Length, opChildren);

            // if this comment is from op, we want to give it a boost, but only if it hasn't had a boost already.
            if (post.UserId == comment.AuthorUserId && !opChildren.Any())
                qa *= 2;

            _commentService.UpdateCommentSorting(comment.Id, confidence, qa);

            if (post.UserId == comment.AuthorUserId)
                // this comment is from the op, then the parent comment's qa sort will be updated because this is an op reply.
                if (comment.ParentId.HasValue)
                    UpdateSortsForComment(comment.ParentId.Value);
        }
    }
}
