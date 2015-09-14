using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.ReadModel
{
    public interface IModeratorWrapper
    {
        List<ModeratorWrapped> Wrap(List<Moderator> moderators, User currentUser = null);

        ModeratorWrapped Wrap(Moderator moderator, User currentUser = null);
    }
}
