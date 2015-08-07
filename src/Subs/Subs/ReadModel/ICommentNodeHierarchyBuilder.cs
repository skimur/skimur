using System;
using System.Collections.Generic;
using Infrastructure.Membership;
using Subs.Services;

namespace Subs.ReadModel
{
    public interface ICommentNodeHierarchyBuilder
    {
        List<CommentNode> Build(CommentTree tree, CommentTreeContext treeContext, User currentUser);

        Dictionary<Guid, CommentNode> WrapComments(List<Guid> comments, User currentUser);
    }

    public class CommentNode
    {
        public CommentNode()
        {
            Children = new List<CommentNode>();
        }

        public Comment Comment { get; set; }

        public List<CommentNode> Children { get; set; }

        public CommentNode Parent { get; set; }

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
