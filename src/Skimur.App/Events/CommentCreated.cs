using System;
using Skimur.Messaging;

namespace Skimur.App.Events
{
    public class CommentCreated : IEvent
    {
        public Guid CommentId { get; set; }

        public Guid PostId { get; set; }
    }
}
