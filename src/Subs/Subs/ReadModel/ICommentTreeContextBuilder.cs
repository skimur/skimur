using System;
using System.Collections.Generic;
using Subs.Services;

namespace Subs.ReadModel
{
    public interface ICommentTreeContextBuilder
    {
        CommentTreeContext Build(CommentTree commentTree, 
            Dictionary<Guid, double> sorter, 
            List<Guid> children = null, 
            Guid? comment = null, 
            int? limit = null, 
            int? maxDepth = null, 
            int context = 0, 
            bool continueThread = true, 
            bool loadMore = true);
    }

    public class CommentTreeContext
    {
        public CommentTreeContext()
        {
            Comments = new List<Guid>();
            TopLevelComments = new List<Guid>();
            TopLevelCandidates = new List<Guid>();
            MoreRecursion = new List<Guid>();
            CommentsChildrenCount = new Dictionary<Guid, int>();
            DontCollapse = new List<Guid>();
        }

        public List<Guid> Comments { get; set; }

        public List<Guid> TopLevelComments { get; set; }

        public List<Guid> TopLevelCandidates { get; set; }

        public List<Guid> MoreRecursion { get; set; }

        public Dictionary<Guid, int> CommentsChildrenCount { get; set; }

        public List<Guid> DontCollapse { get; set; }

        public int OffsetDepth { get; set; }

        public int? MaxDepth { get; set; }

        public CommentSortBy Sort { get; set; }
    }
}
