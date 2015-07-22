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
        public string PostSlug { get; set; }

        public string UserName { get; set; }

        public VoteType? VoteType { get; set; }
    }
}
