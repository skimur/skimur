using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class CreateComment : ICommandReturns<CreateCommentResponse>
    {
        public string PostSlug { get; set; }

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
