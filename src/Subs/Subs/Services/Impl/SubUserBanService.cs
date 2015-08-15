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

        public SubUserBan GetBannedUserInSub(Guid subId, Guid userId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Single<SubUserBan>(x => x.SubId == subId && x.UserId == userId);
            });
        }

        public void BanUserFromSub(Guid subId, Guid userId, string userName, DateTime dateBanned, Guid bannedBy, string reasonPrivate, string reasonPublic)
        {
            _conn.Perform(conn =>
            {
                var existing = conn.Single<SubUserBan>(x => x.UserId == userId && x.SubId == subId);

                if (existing != null)
                {
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

        public void UnBanUserFromSub(Guid subId, Guid userId)
        {
            _conn.Perform(conn => conn.Delete<SubUserBan>(x => x.UserId == userId && x.SubId == subId));
        }

        public void UpdateSubBanForUser(Guid subId, Guid userId, string reason)
        {
            _conn.Perform(conn => conn.Update<SubUserBan>(new { ReasonPrivate = reason }, x => x.SubId == subId && x.UserId == userId));
        }
    }
}
