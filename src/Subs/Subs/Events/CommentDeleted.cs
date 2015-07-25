using System;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class CommentDeleted : IEvent
    {
        public Guid CommentId { get; set; }

        public string PostSlug { get; set; }

        public string SubName { get; set; }

        public string DeletedByUserName { get; set; }
    }
}
