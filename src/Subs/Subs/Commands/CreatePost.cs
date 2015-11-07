using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class CreatePost : ICommandReturns<CreatePostResponse>
    {
        public Guid CreatedByUserId { get; set; }

        public string IpAddress { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public PostType PostType { get; set; }

        public string SubName { get; set; }

        public bool NotifyReplies { get; set; }

        public string Mirror { get; set; }

        public DateTime? OverrideDateCreated { get; set; }
    }

    public class CreatePostResponse
    {
        public Guid? PostId { get; set; }

        public string Error { get; set; }

        public string Title { get; set; }
    }
}
