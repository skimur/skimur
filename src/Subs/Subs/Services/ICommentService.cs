using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface ICommentService
    {
        Comment GetCommentById(Guid id);

        void InsertComment(Comment comment);

        List<Comment> GetAllCommentsForPost(string postSlug);
    }
}
