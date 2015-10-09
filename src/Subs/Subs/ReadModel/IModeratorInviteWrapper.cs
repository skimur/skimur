using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.ReadModel
{
    public interface IModeratorInviteWrapper
    {
        List<ModeratorInviteWrapped> Wrap(List<ModeratorInvite> invites, User currentUser = null);

        ModeratorInviteWrapped Wrap(ModeratorInvite invite, User currentUser = null);
    }
}
