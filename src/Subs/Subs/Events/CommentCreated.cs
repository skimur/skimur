using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class CommentCreated : IEvent
    {
        public Guid CommentId { get; set; }

        public Guid PostId { get; set; }
    }
}
