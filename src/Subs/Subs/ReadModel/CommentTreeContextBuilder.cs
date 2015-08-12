using System;
using System.Collections.Generic;
using System.Linq;
using Skimur.PriorityQueue;
using Subs.Services;

namespace Subs.ReadModel
{
    public class CommentTreeContextBuilder : ICommentTreeContextBuilder
    {
        public CommentTreeContext Build(CommentTree commentTree,
            Dictionary<Guid, double> sorter,
            List<Guid> children = null,
            Guid? comment = null,
            int? limit = null,
            int? maxDepth = null,
            int context = 0,
            bool continueThread = true,
            bool loadMore = true)
        {
            if(children != null && comment != null)
                throw new Exception("You cannot build a tree for both a single comment, and multiple comments.");
            
            var result = new CommentTreeContext();
            result.MaxDepth = maxDepth;

            if (children != null && children.Count == 0)
                return result;

            if (commentTree.CommentIds.Count == 0)
                return result;

            if(sorter == null)
                sorter = new Dictionary<Guid, double>();

            var candidates = new PriorityQueue<CommentQueue>();

            if (children != null)
            {
                UpdateCandidates(candidates, sorter, children.Where(x => commentTree.CommentIds.Contains(x)));
                result.DontCollapse.AddRange(candidates.Select(x => x.CommentId));
            }else if (comment.HasValue)
            {
                var currentComment = (Guid?)comment;
                var path = new List<Guid>();

                while (currentComment.HasValue && path.Count <= context)
                {
                    path.Add(currentComment.Value);
                    currentComment = commentTree.Parents[currentComment.Value];
                }


                result.DontCollapse.AddRange(path);

                foreach (var commentId in path)
                {
                    var parent = commentTree.Parents[commentId];
                    commentTree.Tree[parent ?? Guid.Empty] = new List<Guid> { commentId };
                }
                
                UpdateCandidates(candidates, sorter, new List<Guid> { path[path.Count - 1]});

                result.OffsetDepth = commentTree.Depth[path[path.Count - 1]];
            }
            else
            {
                var topLevelComments = commentTree.Tree.ContainsKey(Guid.Empty)
                    ? commentTree.Tree[Guid.Empty]
                    : new List<Guid>();
                UpdateCandidates(candidates, sorter, topLevelComments);
            }
            
            var items = new List<Guid>();
            while ((!limit.HasValue || items.Count < limit.Value) && candidates.Count > 0)
            {
                var candidate = candidates.Dequeue();

                var commentDepth = commentTree.Depth[candidate.CommentId] - result.OffsetDepth;

                if (!maxDepth.HasValue || commentDepth < maxDepth.Value)
                {
                    items.Add(candidate.CommentId);
                    if (commentTree.Tree.ContainsKey(candidate.CommentId))
                        UpdateCandidates(candidates, sorter, commentTree.Tree[candidate.CommentId]);
                }
                else if (continueThread && commentTree.Parents[candidate.CommentId] != null)
                {
                    var parentId = commentTree.Parents[candidate.CommentId].Value;
                    if (!result.MoreRecursion.Contains(parentId))
                        result.MoreRecursion.Add(parentId);
                }
            }
            
            result.Comments.AddRange(items);
            result.TopLevelComments.AddRange(result.Comments.Where(x => commentTree.Depth[x] == 0));
            result.TopLevelCandidates = candidates.Where(x => ((commentTree.Depth.ContainsKey(x.CommentId) ? commentTree.Depth[x.CommentId] : 0) == 0)).Select(x => x.CommentId).ToList();

            UpdateChildrenCount(result, commentTree, items.Union(result.TopLevelCandidates).ToList());
            
            return result;
        }

        private void UpdateCandidates(PriorityQueue<CommentQueue> candidates, Dictionary<Guid, double> sorter, IEnumerable<Guid> comments)
        {
            foreach (var comment in comments)
                candidates.Enqueue(new CommentQueue(comment, sorter.ContainsKey(comment) ? sorter[comment] : 0));
        }

        private int UpdateChildrenCount(CommentTreeContext context, CommentTree commentTree, List<Guid> comments)
        {
            var childrenCount = comments.Count;

            foreach (var comment in comments)
            {
                if (context.CommentsChildrenCount.ContainsKey(comment))
                    continue;

                if (!commentTree.Tree.ContainsKey(comment))
                {
                    context.CommentsChildrenCount[comment] = 0;
                    continue;
                }

                var count = UpdateChildrenCount(context, commentTree, commentTree.Tree[comment]);

                context.CommentsChildrenCount[comment] = count;

                childrenCount += count;
            }

            return childrenCount;
        }

        class CommentQueue : IComparable<CommentQueue>
        {
            public CommentQueue(Guid commentId, double priority)
            {
                CommentId = commentId;
                Priority = priority;
            }

            public Guid CommentId { get; private set; }

            public double Priority { get; private set; }

            public int CompareTo(CommentQueue other)
            {
                if (Priority < other.Priority) return -1;
                if (Priority > other.Priority) return 1;
                return 0;
            }
        }
    }
}
