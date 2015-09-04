using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class RemovePost : ICommandReturns<RemovePostResponse>
    {
        public Guid PostId { get; set; }

        public Guid RemovedBy { get; set; }
    }

    public class RemovePostResponse
    {
        public string Error { get; set; }
    }
}
