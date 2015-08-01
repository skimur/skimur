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
        private readonly List<Guid> _dontCollapse = new List<Guid>();

        public CommentBuilder(CommentTree commentTree)
        {
            _tree = commentTree;
        }

        public void BuildForTopLevelComments(int? limit, int? maxDepth, bool continueThread = true, bool loadMore = true)
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

                if (!maxDepth.HasValue || commentDepth < maxDepth.Value)
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

        public void BuildForComment(Guid commentId, int? limit, int? maxDepth, int context = 0, bool continueThread = true, bool loadMore = true)
        {
            Clear();

            if (context < 0)
                context = 0;

            var candidates = new HeapPriorityQueue<CommentQueue>(_tree.CommentIds.Count);

            var currentComment = (Guid?)commentId;
            var path = new List<Guid>();
            while (currentComment.HasValue && path.Count <= context)
            {
                path.Add(currentComment.Value);
                currentComment = _tree.Parents[currentComment.Value];
            }

            foreach (var comment in path)
            {
                var parent = _tree.Parents[comment];
                _tree.Tree[parent ?? Guid.Empty] = new List<Guid> {comment};
            }

            _dontCollapse.AddRange(path);
        }

        public List<Guid> Comments { get { return _comments; } }

        public Dictionary<Guid, int> CommentChildrenCount { get { return _commentsChildrenCount; } }

        public List<Guid> DontCollapse { get { return _dontCollapse; } } 

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
        }

        private void Clear()
        {
            _comments.Clear();
            _moreRecursion.Clear();
            _commentsChildrenCount.Clear();
            _dontCollapse.Clear();
        }

        
    }
}
