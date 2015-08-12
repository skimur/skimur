using System;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;

namespace Subs.Services.Impl
{
    public class SubUserBanService : ISubUserBanService
    {
        private readonly IDbConnectionProvider _conn;

        public SubUserBanService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public SeekedList<SubUserBan> GetBannedUsersInSub(Guid subId, string userName = null, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<SubUserBan>().Where(x => x.SubId == subId);

                if (!string.IsNullOrEmpty(userName))
                    query.Where(x => x.UserName.Contains(userName));

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                
                return new SeekedList<SubUserBan>(conn.Select(query), skip ?? 0, take, totalCount);
            });
        }

        public void BanUserFromSub(Guid subId, Guid userId, string userName, DateTime? bannedUntil, DateTime dateBanned, Guid bannedBy, string reasonPrivate, string reasonPublic)
        {
            _conn.Perform(conn =>
            {
                var existing = conn.Single<SubUserBan>(x => x.UserId == userId && x.SubId == subId);

                if (existing != null)
                {
                    existing.BannedUntil = bannedUntil;
                    existing.BannedBy = bannedBy;
                    existing.ReasonPrivate = reasonPrivate;
                    existing.ReasonPublic = reasonPublic;
                    existing.DateBanned = dateBanned;
                    conn.Update(existing);
                }
                else
                {
                    existing = new SubUserBan();
                    existing.Id = GuidUtil.NewSequentialId();
                    existing.BannedUntil = bannedUntil;
                    existing.BannedBy = bannedBy;
                    existing.ReasonPrivate = reasonPrivate;
                    existing.ReasonPublic = reasonPublic;
                    existing.DateBanned = dateBanned;
                    existing.SubId = subId;
                    existing.UserId = userId;
                    existing.UserName = userName;
                    conn.Insert(existing);
                }
            });
        }
    }
}
