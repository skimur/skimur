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

        List<Comment> GetAllCommentsForPost(string postSlug);
    }
}
