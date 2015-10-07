using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class CastVoteForComment : ICommand
    {
        public DateTime DateCasted { get; set; }

        public Guid UserId { get; set; }

        public Guid CommentId { get; set; }

        public VoteType? VoteType { get; set; }

        public string IpAddress { get; set; }
    }
}
