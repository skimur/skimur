using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class ReplyMessage : ICommandReturns<ReplyMessageResponse>
    {
        public Guid ReplyToMessageId { get; set; }

        public Guid Author { get; set; }

        public string AuthorIp { get; set; }
        
        public string Body { get; set; }
    }

    public class ReplyMessageResponse
    {
        public string Error { get; set; }
    }
}
