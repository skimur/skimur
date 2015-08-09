using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Membership;
using Subs.Services;

namespace Subs.ReadModel
{
    public class CommentNodeHierarchyBuilder : ICommentNodeHierarchyBuilder
    {
        private readonly ICommentWrapper _commentWrapper;

        public CommentNodeHierarchyBuilder(ICommentWrapper commentWrapper)
        {
            _commentWrapper = commentWrapper;
        }

        public List<CommentWrapped> Build(CommentTree tree, CommentTreeContext treeContext, User currentUser)
        {
            var wrapped = _commentWrapper.Wrap(treeContext.Comments, currentUser).ToDictionary(x => x.Comment.Id, x => x);
            var final = new List<CommentWrapped>();
            var walked = new List<Guid>();

            foreach (var comment in wrapped.Values)
            {
                comment.NumberOfChildren = treeContext.CommentsChildrenCount[comment.Comment.Id];

                CommentWrapped parent = null;

                if (comment.Comment.ParentId.HasValue)
                    parent = wrapped.ContainsKey(comment.Comment.ParentId.Value)
                        ? wrapped[comment.Comment.ParentId.Value]
                        : null;

                if (comment.Comment.ParentId.HasValue && treeContext.Comments.Contains(comment.Comment.ParentId.Value))
                    comment.IsParentVisible = true;

                if (parent != null && comment.CurrentUserIsAuthor)
                {
                    // this comment is the current user, so lets walk the parents
                    // and uncollapse any comments which may be collapsed
                    var ancestor = parent;
                    while (ancestor != null && !walked.Contains(ancestor.Comment.Id))
                    {
                        ancestor.Collapsed = false;
                        walked.Add(ancestor.Comment.Id);
                        ancestor = (ancestor.Comment.ParentId.HasValue && wrapped.ContainsKey(comment.Comment.ParentId.Value))
                            ? wrapped[comment.Comment.ParentId.Value]
                            : null;
                    }
                }

                comment.MoreRecursion = treeContext.MoreRecursion.Contains(comment.Comment.Id);

                if (parent != null)
                {
                    comment.Parent = comment;
                    parent.Children.Add(comment);
                }
                else
                {
                    if (comment.Comment.ParentId.HasValue)
                    {
                        // we don't have a parent here, but this comment does have a parent.
                        // we need to get it
                        comment.Parent = _commentWrapper.Wrap(comment.Comment.ParentId.Value);
                    }
                    final.Add(comment);
                }
            }

            return final;
        }
    }
}
