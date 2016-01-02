using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IPostWrapper
    {
        List<PostWrapped> Wrap(List<Guid> postIds, User currentUser = null);

        PostWrapped Wrap(Guid postId, User currentUser = null);
    }
}
