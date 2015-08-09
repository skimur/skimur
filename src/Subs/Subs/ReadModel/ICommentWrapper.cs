using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel
{
    public interface ICommentWrapper
    {
        List<CommentWrapped> Wrap(List<Guid> commentIds, User currentUser = null);

        CommentWrapped Wrap(Guid commentId, User currentUser = null);
    }
}
