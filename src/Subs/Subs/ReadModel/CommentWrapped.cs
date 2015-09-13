using System.Collections.Generic;
using Membership;

namespace Subs.ReadModel
{
    public class CommentWrapped
    {
        public CommentWrapped(Comment comment)
        {
            Comment = comment;
        }

        public Comment Comment { get; private set; }
        
        public User Author { get; set; }

        public VoteType? CurrentUserVote { get; set; }

        public Sub Sub { get; set; }
        
        public int Score { get; set; }

        public bool CurrentUserIsAuthor { get; set; }
        
        public Post Post { get; set; }

        public bool CanReport { get; set; }

        public bool CanManage { get; set; }

        public bool CanDelete { get; set; }

        public bool CanEdit { get; set; }

        public List<ReportSummary> Reports { get; set; } 
    }
}
