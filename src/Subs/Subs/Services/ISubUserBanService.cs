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

        void BanUserFromSub(Guid subId, Guid userId, string userName, DateTime? bannedUntil, DateTime dateBanned, Guid bannedBy, string reasonPrivate, string reasonPublic);
    }
}
