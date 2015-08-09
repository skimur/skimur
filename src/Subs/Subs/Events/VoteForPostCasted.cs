using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class VoteForPostCasted : IEvent
    {
        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

        public VoteType? VoteType { get; set; }
    }
}
