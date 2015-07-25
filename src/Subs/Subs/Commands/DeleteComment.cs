using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class DeleteComment : ICommandReturns<DeleteCommentResponse>
    {
        public Guid CommentId { get; set; }

        public string UserName { get; set; }

        public DateTime DateDeleted { get; set; }
    }

    public class DeleteCommentResponse
    {
        public string Error { get; set; }
    }
}
