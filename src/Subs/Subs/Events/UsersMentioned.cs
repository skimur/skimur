using System;
using System.Collections.Generic;
using Skimur.Messaging;

namespace Subs.Events
{
    public class UsersMentioned : IEvent
    {
        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public List<string> Users { get; set; } 
    }
}
