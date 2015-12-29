using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class CreateComment : ICommandReturns<CreateCommentResponse>
    {
        public Guid PostId { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime DateCreated { get; set; }

        public string AuthorUserName { get; set; }

        public string AuthorIpAddress { get; set; }

        public string Body { get; set; }

        public bool SendReplies { get; set; }
    }

    public class CreateCommentResponse
    {
        public string Error { get; set; }

        public Guid? CommentId { get; set; }
    }
}
