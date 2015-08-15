using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface ISubUserBanService
    {
        SeekedList<SubUserBan> GetBannedUsersInSub(Guid subId, string userName = null, int? skip = null, int? take = null);

        SubUserBan GetBannedUserInSub(Guid subId, Guid userId);

        void BanUserFromSub(Guid subId, Guid userId, string userName, DateTime dateBanned, Guid bannedBy, string reasonPrivate, string reasonPublic);

        void UnBanUserFromSub(Guid subId, Guid userId);

        void UpdateSubBanForUser(Guid subId, Guid userId, string reason);
    }
}
