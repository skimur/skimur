using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class MarkMessagesAsRead : ICommand
    {
        public Guid UserId { get; set; }

        public List<Guid> Messages { get; set; } 
    }
}
