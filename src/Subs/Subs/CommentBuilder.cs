using Subs.ReadModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur.PriorityQueue;

namespace Subs
{
    public abstract class CommentBuilder<T> where T : ICommentTreeNode<T>
    {
        readonly CommentTree _tree;

        protected CommentBuilder(CommentTree commentTree)
        {
            _tree = commentTree;
        }

        public List<ICommentTreeNode<T>> GetTopLevelComments(int? limit, int maxDepth)
        {
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
            }

            var nodes = BuildNode(items);

            foreach (var node in nodes)
            {

            }

            return nodes;
        }

        private void UpdateCandidates(HeapPriorityQueue<CommentQueue> candidates, Dictionary<Guid, double> sorter, List<Guid> comments)
        {
            foreach (var comment in comments.Where(sorter.ContainsKey))
                candidates.Enqueue(new CommentQueue(comment), sorter[comment]);
        }

        public class CommentQueue : PriorityQueueNode
        {
            public CommentQueue(Guid commentId)
            {
                CommentId = commentId;
            }

            public Guid CommentId { get; private set; }
        }

        protected abstract List<ICommentTreeNode<T>> BuildNode(List<Guid> commentIds);
    }

    public interface ICommentTreeNode<T> where T : ICommentTreeNode<T>
    {
        Guid Id { get; }

        List<ICommentTreeNode<T>> Children { get; set; }
    }
}
