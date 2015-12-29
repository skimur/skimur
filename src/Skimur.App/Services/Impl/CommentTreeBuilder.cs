using System;
using System.Collections.Generic;
using System.Linq;

namespace Subs.Services.Impl
{
    public class CommentTreeBuilder : ICommentTreeBuilder
    {
        private readonly ICommentService _commentService;

        public CommentTreeBuilder(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public CommentTree GetCommentTree(Guid postId)
        {
            // TODO: this method is very db intensive. however, it should be easy to improve the performance once a caching strategy is implemented.

            var comments = _commentService.GetAllCommentsForPost(postId);

            var tree = new CommentTree
            {
                PostId = postId,
                CommentIds = comments.Select(x => x.Id).ToList(),
                Parents = comments.ToDictionary(x => x.Id, x => x.ParentId),
                Tree = new Dictionary<Guid, List<Guid>>(),
                Depth = comments.ToDictionary(x => x.Id, x => 0)
            };

            foreach (var comment in comments)
            {
                List<Guid> children;

                var parentId = comment.ParentId ?? Guid.Empty;

                if (tree.Tree.ContainsKey(parentId))
                    children = tree.Tree[parentId];
                else
                {
                    children = new List<Guid>();
                    tree.Tree[parentId] = children;
                }

                children.Add(comment.Id);
            }
            
            foreach (var comment in comments)
            {
                if (!comment.ParentId.HasValue)
                {
                    tree.Depth[comment.Id] = 0;
                    continue;
                }
                
                var parentId = comment.ParentId;
                var depth = 0;
                while (parentId.HasValue)
                {
                    depth++;
                    parentId = tree.Parents[parentId.Value];
                }
                tree.Depth[comment.Id] = depth;
            }

            return tree;
        }
    }
}
