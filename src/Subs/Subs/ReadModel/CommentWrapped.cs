using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel
{
    public class CommentWrapped
    {
        public CommentWrapped(Comment comment)
        {
            Comment = comment;
            Children = new List<CommentWrapped>();
        }

        public Comment Comment { get; private set; }

        public List<CommentWrapped> Children { get; set; }

        public CommentWrapped Parent { get; set; }

        public User Author { get; set; }

        public VoteType? CurrentUserVote { get; set; }

        public Sub Sub { get; set; }

        public int NumberOfChildren { get; set; }

        public bool Collapsed { get; set; }

        public int Score { get; set; }

        public bool CurrentUserIsAuthor { get; set; }

        public bool CanDelete { get; set; }

        public bool CanEdit { get; set; }

        public bool MoreRecursion { get; set; }

        public Post Post { get; set; }

        public bool IsParentVisible { get; set; }
    }
}
