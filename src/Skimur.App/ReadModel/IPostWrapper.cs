using System;
using System.Collections.Generic;
using Membership;

namespace Subs.ReadModel
{
    public interface IPostWrapper
    {
        List<PostWrapped> Wrap(List<Guid> postIds, User currentUser = null);

        PostWrapped Wrap(Guid postId, User currentUser = null);
    }
}
