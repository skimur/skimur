using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using ServiceStack.Commands;

namespace Subs.Commands
{
    public class SendMessage : ICommandReturns<SendMessageResponse>
    {
        public Guid Author { get; set; }

        public string AuthorIp { get; set; }

        public Guid? SendAsSub { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }

    public class SendMessageResponse
    {
        public string Error { get; set; }
    }
}
