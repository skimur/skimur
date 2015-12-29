using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public class PostWrapped
    {
        public PostWrapped(Post post)
        {
            Post = post;
        }

        public Post Post { get; private set; }

        public Sub Sub { get; set; }

        public User Author { get; set; }
        
        public VoteType? CurrentUserVote;
        
        public Verdict? Verdict { get; set; }

        public bool CanManage { get; set; }

        public bool CanReport { get; set; }

        public List<ReportSummary> Reports { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanSticky { get; set; }
    }
}
