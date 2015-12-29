using System;
using System.Collections.Generic;

namespace Skimur.App.Services
{
    public interface ICommentTreeBuilder
    {
        CommentTree GetCommentTree(Guid postId);
    }

    public class CommentTree
    {
        public Guid PostId { get; set; }

        public List<Guid> CommentIds { get; set; }

        public Dictionary<Guid, int> Depth { get; set; }

        public Dictionary<Guid, Guid?> Parents { get; set; }

        public Dictionary<Guid, List<Guid>> Tree { get; set; }
    }
}
