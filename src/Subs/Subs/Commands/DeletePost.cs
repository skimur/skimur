using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class DeletePost : ICommandReturns<DeletePostResponse>
    {
        public Guid PostId { get; set; }

        public Guid DeleteBy { get; set; }

        public string Reason { get; set; }
    }

    public class DeletePostResponse
    {
        public string Error { get; set; }
    }
}
