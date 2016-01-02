using System;
using System.Collections.Generic;

namespace Skimur.App.Services
{
    public interface IModerationService
    {
        List<Moderator> GetAllModsForSub(Guid subId);

        void AddModToSub(Guid userId, Guid subId, ModeratorPermissions permissions, Guid? addedBy = null);

        void RemoveModFromSub(Guid userId, Guid subId);

        List<Guid> GetSubsModeratoredByUser(Guid userId);

        Dictionary<Guid, ModeratorPermissions> GetSubsModeratoredByUserWithPermissions(Guid userId);

        ModeratorPermissions? GetUserPermissionsForSub(User user, Guid subId);

        Moderator GetModeratorInfoForUserInSub(Guid userId, Guid subId);

        void UpdateUserModPermissionForSub(Guid userId, Guid subId, ModeratorPermissions permissions);
    }
}
