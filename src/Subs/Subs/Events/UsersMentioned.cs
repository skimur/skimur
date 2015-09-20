using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class UsersMentioned : IEvent
    {
        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public List<string> Users { get; set; } 
    }
}
