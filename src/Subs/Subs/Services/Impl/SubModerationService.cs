using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Utils;
using Membership;
using ServiceStack.OrmLite;
using Skimur;

namespace Subs.Services.Impl
{
    public class SubModerationService : ISubModerationService
    {
        private readonly IDbConnectionProvider _conn;

        public SubModerationService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public bool CanUserModerateSub(Guid userId, Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Count(conn.From<Moderator>().Where(x => x.UserId == userId && x.SubId == subId)) > 0;
            });
        }

        public List<Guid> GetAllModsForSub(Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<Moderator>().Where(x => x.SubId == subId).Select(x => x.UserId))
                    .Select(x => x.UserId).ToList();
            });
        }

        public void AddModToSub(Guid userId, Guid subId, Guid? addedBy = null)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count<Moderator>(x => x.UserId == userId && x.SubId == subId) > 0)
                    return;

                conn.Insert(new Moderator
                {
                    Id = GuidUtil.NewSequentialId(),
                    UserId = userId,
                    SubId = subId,
                    AddedOn = Common.CurrentTime(),
                    AddedBy = addedBy,
                    Permissions = ModeratorPermissions.All
                });
            });
        }

        public void RemoveModFromSub(Guid userId, Guid subId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Moderator>(x => x.UserId == userId && x.SubId == subId);
            });
        }

        public List<Guid> GetSubsModeratoredByUser(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Moderator>();
                query.Where(x => x.UserId == userId);
                query.SelectExpression = "SELECT \"sub_id\"";
                return conn.Select(query).Select(x => x.SubId).ToList();
            });
        }

        public ModeratorPermissions? GetUserPermissionsForSub(User user, Guid subId)
        {
            if (user == null) return null;
            if(user.IsAdmin) return ModeratorPermissions.All;

            return _conn.Perform(conn =>
            {
                var query = conn.From<Moderator>().Where(x => x.UserId == user.Id && x.SubId == subId);
                query.SelectExpression = "SELECT \"permissions\"";
                var result = conn.Single(query);
                if (result == null)
                    return (ModeratorPermissions?)null;
                return result.Permissions;
            });
        }
    }
}
