using System;
using Skimur.Messaging;

namespace Skimur.App.Events
{
    public class CommentDeleted : IEvent
    {
        public Guid CommentId { get; set; }

        public Guid PostId { get; set; }

        public Guid SubId { get; set; }

        public Guid DeletedByUserId { get; set; }
    }
}
