using System;
using Skimur.Messaging;

namespace Subs.Events
{
    public class CommentCreated : IEvent
    {
        public Guid CommentId { get; set; }

        public Guid PostId { get; set; }
    }
}
