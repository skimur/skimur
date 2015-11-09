using System;
using Skimur.Messaging;

namespace Subs.Events
{
    public class VoteForPostCasted : IEvent
    {
        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

        public VoteType? PreviousVote { get; set; }

        public VoteType? VoteType { get; set; }
    }
}
