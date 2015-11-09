using System;
using Skimur.Messaging;

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
