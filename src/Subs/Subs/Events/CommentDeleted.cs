using System;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class CommentDeleted : IEvent
    {
        public Guid CommentId { get; set; }

        public Guid PostId { get; set; }

        public Guid SubId { get; set; }

        public Guid DeletedByUserId { get; set; }
    }
}
