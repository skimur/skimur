using System;

namespace Skimur.App.Services
{
    public interface ISubUserBanService
    {
        SeekedList<SubUserBan> GetBannedUsersInSub(Guid subId, string userName = null, int? skip = null, int? take = null);

        SubUserBan GetBannedUserInSub(Guid subId, Guid userId);

        bool IsUserBannedFromSub(Guid subId, Guid userId);

        void BanUserFromSub(Guid subId, Guid userId, string userName, DateTime dateBanned, Guid bannedBy, string reasonPrivate, string reasonPublic);

        void UnBanUserFromSub(Guid subId, Guid userId);

        void UpdateSubBanForUser(Guid subId, Guid userId, string reason);
    }
}
