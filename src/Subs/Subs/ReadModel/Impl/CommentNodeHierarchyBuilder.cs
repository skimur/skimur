using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Membership;
using Subs.Services;

namespace Subs.ReadModel.Impl
{
    public class CommentNodeHierarchyBuilder : ICommentNodeHierarchyBuilder
    {
        private readonly ICommentWrapper _commentWrapper;

        public CommentNodeHierarchyBuilder(ICommentWrapper commentWrapper)
        {
            _commentWrapper = commentWrapper;
        }

        public List<ICommentNode> Build(CommentTree tree, CommentTreeContext treeContext, User currentUser)
        {
            var wrapped = _commentWrapper.Wrap(treeContext.Comments, currentUser).ToDictionary(x => x.Comment.Id, x => new CommentNode(x));
            var final = new List<ICommentNode>();
            var walked = new List<Guid>();

            // lets mark any comments as collapsed if needed
            foreach (var comment in wrapped.Values)
            {
                // TODO: make this configurable per-user
                int minimumScore = 0;
                if ((comment.Comment.Author != null && currentUser != null) && currentUser.Id == comment.Comment.Author.Id)
                {
                    // the current user is the author, don't collapse!
                    comment.Collapsed = false;
                }
                else if (comment.Comment.Score < minimumScore)
                {
                    // too many down votes to show to the user
                    comment.Collapsed = true;
                }
                else
                {
                    // the current user is not the author, and we have enough upvotes to display,
                    // don't collapse
                    comment.Collapsed = false;
                }
            }

            foreach (var comment in wrapped.Values)
            {
                comment.NumberOfChildren = treeContext.CommentsChildrenCount[comment.Comment.Comment.Id];

                CommentNode parent = null;

                if (comment.Comment.Comment.ParentId.HasValue)
                    parent = wrapped.ContainsKey(comment.Comment.Comment.ParentId.Value)
                        ? wrapped[comment.Comment.Comment.ParentId.Value]
                        : null;

                if (comment.Comment.Comment.ParentId.HasValue && treeContext.Comments.Contains(comment.Comment.Comment.ParentId.Value))
                    comment.IsParentVisible = true;

                if (parent != null && comment.Comment.CurrentUserIsAuthor)
                {
                    // this comment is the current user, so lets walk the parents
                    // and uncollapse any comments which may be collapsed
                    var ancestor = parent;
                    while (ancestor != null && !walked.Contains(ancestor.Comment.Comment.Id))
                    {
                        ancestor.Collapsed = false;
                        walked.Add(ancestor.Comment.Comment.Id);
                        ancestor = (ancestor.Comment.Comment.ParentId.HasValue && wrapped.ContainsKey(comment.Comment.Comment.ParentId.Value))
                            ? wrapped[comment.Comment.Comment.ParentId.Value]
                            : null;
                    }
                }

                if (treeContext.MoreRecursion.Contains(comment.Comment.Comment.Id))
                    comment.Children.Add(new MoreRecursionNode(comment.Comment));

                if (parent != null)
                {
                    comment.Parent = comment.Comment;
                    parent.Children.Add(comment);
                }
                else
                {
                    if (comment.Comment.Comment.ParentId.HasValue)
                    {
                        // we don't have a parent here, but this comment does have a parent.
                        // we need to get it
                        comment.Parent = _commentWrapper.Wrap(comment.Comment.Comment.ParentId.Value);
                    }
                    final.Add(comment);
                }
            }

            foreach (var visibleId in wrapped.Keys)
            {
                // this item is already scheduled to have a new link to see all the children
                if (treeContext.MoreRecursion.Contains(visibleId)) continue;

                var children = tree.Tree.ContainsKey(visibleId) ? tree.Tree[visibleId] : new List<Guid>();
                var missingChildren = children.Where(x => !treeContext.Comments.Contains(x)).ToList();

                if (missingChildren.Count > 0)
                {
                    var visibleChildren = children.Where(x => treeContext.Comments.Contains(x)).ToList();
                    var visibleCount = visibleChildren.Sum(x => (treeContext.CommentsChildrenCount[x]) + 1);
                    var missingCount = treeContext.CommentsChildrenCount[visibleId] - visibleCount;
                    var missingDepth = (tree.Depth.ContainsKey(visibleId) ? tree.Depth[visibleId] : 0) + 1 -
                                       treeContext.OffsetDepth;
                    
                    var moreChildren = new MoreChildren();
                    moreChildren.ChildComments.AddRange(missingChildren);
                    moreChildren.MissingCount = missingCount;
                    moreChildren.PostId = tree.PostId;
                    moreChildren.Sort = treeContext.Sort;
                    moreChildren.Depth = missingDepth;
                    wrapped[visibleId].Children.Add(moreChildren);
                }
            }

            if (treeContext.TopLevelCandidates.Count > 0)
            {
                var moreChildren = new MoreChildren();
                moreChildren.ChildComments.AddRange(treeContext.TopLevelCandidates);
                moreChildren.MissingCount = moreChildren.ChildComments.Sum(x => (treeContext.CommentsChildrenCount.ContainsKey(x) ? treeContext.CommentsChildrenCount[x] : 0) + 1);
                moreChildren.PostId = tree.PostId;
                moreChildren.Sort = treeContext.Sort;
                moreChildren.Depth = 0;
                final.Add(moreChildren);
            }

            return final;
        }
    }
}
