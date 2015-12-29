using System;
using Skimur.Messaging;

namespace Subs.Events
{
    public class SubScriptionChanged : IEvent
    {
        public bool Subcribed { get; set; }

        public bool Unsubscribed { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }
    }
}
