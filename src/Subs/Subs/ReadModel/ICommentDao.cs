using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface ICommentDao
    {
        Comment GetCommentById(Guid id);

        List<Comment> GetAllCommentsForPost(string postSlug, CommentSortBy? sortBy = null);

        CommentTree GetCommentTree(string postSlug, CommentSortBy? sortBy = null);
    }

    public class CommentTree
    {
        public List<Guid> CommentIds { get; set; }

        public Dictionary<Guid, int> Depth { get; set; }

        public Dictionary<Guid, Guid?> Parents { get; set; }

        public Dictionary<Guid, List<Guid>> Tree { get; set; }

        public Dictionary<Guid, double> Sorter { get; set; }
    }

    public enum CommentSortBy
    {
        Best,
        Top,
        New,
        Controversial,
        Old,
        Qa
    }
}
