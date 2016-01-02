using System;

namespace Skimur.App.ReadModel
{
    public interface ISubUserBanDao
    {
        SeekedList<SubUserBan> GetBannedUsersInSub(Guid subId, string userName = null, int? skip = null, int? take = null);

        SubUserBan GetBannedUserInSub(Guid subId, Guid userId);

        bool IsUserBannedFromSub(Guid subId, Guid userId);
    }
}
