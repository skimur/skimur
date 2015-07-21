using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class EditComment : ICommandReturns<EditCommentResponse>
    {
        public DateTime DateEdited { get; set; }

        public Guid CommentId { get; set; }

        public string Body { get; set; }
    }

    public class EditCommentResponse
    {
        public string Error { get; set; }

        public string Body { get; set; }

        public string FormattedBody { get; set; }
    }
}
