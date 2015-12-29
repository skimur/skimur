using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
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
