using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector.Diagnostics;

namespace Subs.Services
{
    public interface IModerationInviteService
    {
        void AddInvite(Guid userId, Guid subId, Guid? invitedBy, ModeratorPermissions permissions);

        void UpdateInvitePermissions(Guid userId, Guid subId, ModeratorPermissions permissions);

        void RemoveModeratorInvite(Guid userId, Guid subId);

        ModeratorInvite GetModeratorInviteInfo(Guid userId, Guid subId);

        List<ModeratorInvite> GetModeratorInvitesForSub(Guid subId);
    }
}
