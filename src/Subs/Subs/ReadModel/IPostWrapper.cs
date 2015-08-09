using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel
{
    public interface IPostWrapper
    {
        List<PostWrapped> Wrap(List<Guid> postIds, User currentUser = null);

        PostWrapped Wrap(Guid postId, User currentUser = null);
    }
}
