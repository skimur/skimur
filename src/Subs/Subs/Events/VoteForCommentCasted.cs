using System;
using Skimur.Messaging;

namespace Subs.Events
{
    public class VoteForCommentCasted : IEvent
    {
        public Guid CommentId { get; set; }

        public string UserName { get; set; }

        public VoteType? PreviousVote { get; set; }

        public VoteType? VoteType { get; set; }
    }
}
