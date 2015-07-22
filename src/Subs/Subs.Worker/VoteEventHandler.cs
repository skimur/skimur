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
    public class VoteEventHandler : 
        IEventHandler<VoteForPostCasted>,
        IEventHandler<VoteForCommentCasted>
    {
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly ICommentService _commentService;

        public VoteEventHandler(IPostService postService, IVoteService voteService, ICommentService commentService)
        {
            _postService = postService;
            _voteService = voteService;
            _commentService = commentService;
        }

        public void Handle(VoteForPostCasted @event)
        {
            // NOTE: these event should be raised once for every exact vote. 
            // we could possible simple value+1 or value-1 in the query if we need to for the post, instead of quering the actual number of votes for a post.

            int upVotes;
            int downVotes;
            _voteService.GetTotalVotesForPost(@event.PostSlug, out upVotes, out downVotes);

            _postService.UpdatePostVotes(@event.PostSlug, upVotes, downVotes);
        }

        public void Handle(VoteForCommentCasted @event)
        {
            // NOTE: these event should be raised once for every exact vote. 
            // we could possible simple value+1 or value-1 in the query if we need to for the comment, instead of quering the actual number of votes for a comment.

            int upVotes;
            int downVotes;
            _voteService.GetTotalVotesForComment(@event.CommentId, out upVotes, out downVotes);

            _commentService.UpdateCommentVotes(@event.CommentId, upVotes, downVotes);
        }
    }
}
