using Subs;

namespace Skimur.Web.Models
{
    public class CreatePostViewModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public PostType PostType { get; set; }

        public string SubName { get; set; }

        public bool NotifyReplies { get; set; }
    }
}
