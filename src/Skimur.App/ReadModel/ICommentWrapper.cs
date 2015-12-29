using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface ICommentWrapper
    {
        List<CommentWrapped> Wrap(List<Guid> commentIds, User currentUser = null);

        CommentWrapped Wrap(Guid commentId, User currentUser = null);
    }
}
