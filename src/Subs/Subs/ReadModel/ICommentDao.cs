using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs.Services;

namespace Subs.ReadModel
{
    public interface ICommentDao
    {
        Comment GetCommentById(Guid id);

        List<Comment> GetAllCommentsForPost(string postSlug, CommentSortBy? sortBy = null);

        CommentTree GetCommentTree(string postSlug);

        Dictionary<Guid, double> GetCommentTreeSorter(string postSlug, CommentSortBy sortBy);

        CommentTreeContext BuildCommentTreeContext(CommentTree commentTree, Dictionary<Guid, double> sorter, List<Guid> children = null, Guid? comment = null);
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
