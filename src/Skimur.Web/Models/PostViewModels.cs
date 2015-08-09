using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class CreatePostModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public PostType PostType { get; set; }

        public string SubName { get; set; }

        public bool NotifyReplies { get; set; }
    }
    
    public class PostDetailsModel
    {
        public PostWrapped Post { get; set; }

        public SubWrapped Sub { get; set; }

        public CommentListModel Comments { get; set; }
    }
}
