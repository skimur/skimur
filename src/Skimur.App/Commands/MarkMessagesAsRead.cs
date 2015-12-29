using System;
using System.Collections.Generic;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class MarkMessagesAsRead : ICommand
    {
        public Guid UserId { get; set; }

        public List<Guid> Messages { get; set; } 
    }
}
