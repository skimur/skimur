using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
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

        public Guid MessageId { get; set; }
    }
}
