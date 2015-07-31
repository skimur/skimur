using Subs.ReadModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using Skimur.PriorityQueue;

namespace Subs
{
    public class CommentBuilder
    {
        readonly CommentTree _tree;
        private readonly List<Guid> _comments = new List<Guid>();
        private readonly List<Guid> _topLevelComments = new List<Guid>();
        private readonly List<Guid> _moreRecursion = new List<Guid>();
        private readonly Dictionary<Guid, int> _commentsChildrenCount = new Dictionary<Guid, int>();

        public CommentBuilder(CommentTree commentTree)
        {
            _tree = commentTree;
        }

        public void BuildForTopLevelComments(int? limit, int maxDepth, bool continueThread = true, bool loadMore = true)
        {
            Clear();

            var candidates = new HeapPriorityQueue<CommentQueue>(_tree.CommentIds.Count);

            var topComments = _tree.Tree.ContainsKey(Guid.Empty) ? _tree.Tree[Guid.Empty] : new List<Guid>();

            UpdateCandidates(candidates, _tree.Sorter, topComments);

            var items = new List<Guid>();
            while ((!limit.HasValue || items.Count < limit.Value) && candidates.Count > 0)
            {
                var candidate = candidates.Dequeue();

                var commentDepth = _tree.Depth[candidate.CommentId];

                if (commentDepth < maxDepth)
                {
                    items.Add(candidate.CommentId);
                    if (_tree.Tree.ContainsKey(candidate.CommentId))
                        UpdateCandidates(candidates, _tree.Sorter, _tree.Tree[candidate.CommentId]);
                }
                else if (continueThread && _tree.Parents[candidate.CommentId] != null)
                {
                    var parentId = _tree.Parents[candidate.CommentId].Value;
                    if (!_moreRecursion.Contains(parentId))
                        _moreRecursion.Add(parentId);
                }
            }

            _comments.AddRange(items);
            _topLevelComments.AddRange(_comments.Where(x => _tree.Depth[x] == 0));

            UpdateChildrenCount(_topLevelComments);

            if (!loadMore)
                return;

            return;

            foreach (var visibleComment in _comments)
            {
                if (_moreRecursion.Contains(visibleComment))
                    continue;

                var children = _tree.Tree[visibleComment];

                var missingChildren = children.Except(_comments).ToList();

                if (missingChildren.Count > 0)
                {
                    var visibleChildren = children.Intersect(_comments);
                }
            }
        }

        public List<Guid> Comments { get { return _comments; } }

        public Dictionary<Guid, int> CommentChildrenCount { get { return _commentsChildrenCount; } }

        public List<Guid> TopLevelComments { get { return _topLevelComments; } }

        public List<Guid> MoreRecursion { get { return _moreRecursion; } }

        private void UpdateCandidates(HeapPriorityQueue<CommentQueue> candidates, Dictionary<Guid, double> sorter, List<Guid> comments)
        {
            foreach (var comment in comments.Where(sorter.ContainsKey))
                candidates.Enqueue(new CommentQueue(comment), sorter[comment]);
        }

        private int UpdateChildrenCount(List<Guid> comments)
        {
            var childrenCount = comments.Count;

            foreach (var comment in comments)
            {
                if(_commentsChildrenCount.ContainsKey(comment))
                    continue;

                if (!_tree.Tree.ContainsKey(comment))
                {
                    _commentsChildrenCount[comment] = 0;
                    continue;
                }

                var count = UpdateChildrenCount(_tree.Tree[comment]);

                _commentsChildrenCount[comment] = count;

                childrenCount += count;
            }

            return childrenCount;

            //while (stack.Count > 0)
            //{
            //    var current = stack[stack.Count - 1];

            //    if (_commentsChildrenCount.ContainsKey(current))
            //    {
            //        stack.RemoveAt(stack.Count - 1);
            //        continue;
            //    }

            //    var children = _tree.Tree.ContainsKey(current) ? _tree.Tree[current] : new List<Guid>();

            //    foreach (var child in children)
            //    {
            //        if (!_commentsChildrenCount.ContainsKey(child) && _tree.Tree.ContainsKey(child))
            //            _commentsChildrenCount[child] = 0;
            //    }

            //    var missing = children.Except(_commentsChildrenCount.Keys).ToList();

            //    if (missing.Count == 0)
            //    {
            //        _commentsChildrenCount[current] = 0;
            //        stack.RemoveAt(stack.Count - 1);
            //        foreach (var child in children)
            //        {
            //            _commentsChildrenCount[current] += 1 + _commentsChildrenCount[child];
            //        }
            //    }
            //    else
            //    {
            //        stack.AddRange(missing);
            //    }
            //}
        }

        private void Clear()
        {
            _comments.Clear();
            _moreRecursion.Clear();
            _commentsChildrenCount.Clear();
        }

        class CommentQueue : PriorityQueueNode
        {
            public CommentQueue(Guid commentId)
            {
                CommentId = commentId;
            }

            public Guid CommentId { get; private set; }
        }
    }
}
