using System;
using System.Collections.Generic;
using Infrastructure.Membership;
using Subs.Services;

namespace Subs.ReadModel
{
    public interface ICommentNodeHierarchyBuilder
    {
        List<ICommentNode> Build(CommentTree tree, CommentTreeContext treeContext, User currentUser);
    }

    public enum NodeType
    {
        Comment,
        MoreChildren,
        MoreRecursion
    }

    public interface ICommentNode
    {
        NodeType NodeType { get; }

        List<ICommentNode> Children { get; }
    }
    
    public class CommentNode : ICommentNode
    {
        public CommentNode(CommentWrapped commentWrapped)
        {
            Comment = commentWrapped;
            Children = new List<ICommentNode>();
        }

        public CommentWrapped Comment { get; set; }

        public CommentWrapped Parent { get; set; }

        public int NumberOfChildren { get; set; }
        
        public bool Collapsed { get; set; }

        public bool IsParentVisible { get; set; }

        #region INode

        public NodeType NodeType
        {
            get { return NodeType.Comment; }
        }

        public List<ICommentNode> Children { get; set; }
        
        #endregion
    }

    public class MoreRecursionNode : ICommentNode
    {
        public MoreRecursionNode(CommentWrapped commentWrapped)
        {
            Comment = commentWrapped;
            Children = new List<ICommentNode>();
        }

        public CommentWrapped Comment { get; set; }
        
        #region INode

        public NodeType NodeType
        {
            get { return NodeType.MoreRecursion; }
        }

        public List<ICommentNode> Children { get; set; }

        #endregion
    }

    public class MoreChildren : ICommentNode
    {
        public MoreChildren()
        {
            ChildComments = new List<Guid>();
        }

        public List<Guid> ChildComments { get; set; }

        public int MissingCount { get; set; }

        public int Depth { get; set; }

        public CommentSortBy Sort { get; set; }
        
        public Guid PostId { get; set; }

        #region INode

        public NodeType NodeType
        {
            get { return NodeType.MoreChildren; }
        }

        public List<ICommentNode> Children { get; set; }

        #endregion
    }
}
