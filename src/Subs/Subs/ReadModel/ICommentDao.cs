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

        List<Comment> GetCommentsByIds(List<Guid> ids);
            
        List<Comment> GetAllCommentsForPost(string postSlug, CommentSortBy? sortBy = null);

        CommentTree GetCommentTree(string postSlug);

        Dictionary<Guid, double> GetCommentTreeSorter(string postSlug, CommentSortBy sortBy);
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
