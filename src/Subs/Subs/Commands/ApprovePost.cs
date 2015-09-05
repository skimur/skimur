using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class ApprovePost : ICommandReturns<ApprovePostResponse>
    {
        public Guid PostId { get; set; }

        public Guid ApprovedBy { get; set; }
    }

    public class ApprovePostResponse
    {
        public string Error { get; set; }
    }
}
