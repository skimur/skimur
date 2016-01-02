using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IModeratorInviteWrapper
    {
        List<ModeratorInviteWrapped> Wrap(List<ModeratorInvite> invites, User currentUser = null);

        ModeratorInviteWrapped Wrap(ModeratorInvite invite, User currentUser = null);
    }
}
