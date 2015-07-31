using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface ICommentTreeBuilder
    {
        CommentTree GetCommentTree(string postSlug, CommentSortBy? sortBy = null);
    }
}
