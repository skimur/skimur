using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class EditPostContent : ICommandReturns<EditPostContentResponse>
    {
        public Guid EditedBy { get; set; }

        public Guid PostId { get; set; }

        public string Content { get; set; }
    }

    public class EditPostContentResponse
    {
        public string Error { get; set; }

        public string Content { get; set; }

        public string ContentFormatted { get; set; }
    }
}
