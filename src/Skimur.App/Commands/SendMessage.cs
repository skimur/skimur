using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class SendMessage : ICommandReturns<SendMessageResponse>
    {
        public SendMessage()
        {
            Type = MessageType.Private;
        }

        public Guid Author { get; set; }

        public string AuthorIp { get; set; }

        public Guid? SendAsSub { get; set; }

        public string To { get; set; }

        public Guid? ToUserId { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public MessageType Type { get; set; }

        public Guid? CommentId { get; set; }

        public Guid? PostId { get; set; }
    }

    public class SendMessageResponse
    {
        public string Error { get; set; }

        public Guid MessageId { get; set; }
    }
}
