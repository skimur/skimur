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
            
        List<Comment> GetAllCommentsForPost(Guid postId, CommentSortBy? sortBy = null);
        
        Dictionary<Guid, double> GetCommentTreeSorter(Guid postId, CommentSortBy sortBy);

        CommentTree GetCommentTree(Guid postId);
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
