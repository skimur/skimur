using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IModeratorWrapper
    {
        List<ModeratorWrapped> Wrap(List<Moderator> moderators, User currentUser = null);

        ModeratorWrapped Wrap(Moderator moderator, User currentUser = null);
    }
}
