using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Utils;

namespace Skimur.App.Services.Impl
{
    public class ModerationInviteService : IModerationInviteService
    {
        private readonly IDbConnectionProvider _conn;

        public ModerationInviteService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void AddInvite(Guid userId, Guid subId, Guid? invitedBy, ModeratorPermissions permissions)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count<ModeratorInvite>(x => x.UserId == userId && x.SubId == subId) == 0)
                {
                    conn.Insert(new ModeratorInvite
                    {
                        Id = GuidUtil.NewSequentialId(),
                        UserId = userId,
                        SubId = subId,
                        InvitedBy = invitedBy,
                        InvitedOn = Common.CurrentTime(),
                        Permissions = permissions
                    });
                }
                // update permissions if invite already present?
            });
        }

        public void UpdateInvitePermissions(Guid userId, Guid subId, ModeratorPermissions permissions)
        {
            _conn.Perform(conn =>
            {
                conn.Update<ModeratorInvite>(new { Permissions = permissions }, x => x.UserId == userId && x.SubId == subId);
            });
        }

        public void RemoveModeratorInvite(Guid userId, Guid subId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<ModeratorInvite>(x => x.UserId == userId && x.SubId == subId);
            });
        }

        public ModeratorInvite GetModeratorInviteInfo(Guid userId, Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Single<ModeratorInvite>(x => x.UserId == userId && x.SubId == subId);
            });
        }

        public List<ModeratorInvite> GetModeratorInvitesForSub(Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<ModeratorInvite>().Where(x => x.SubId == subId).OrderBy(x => x.InvitedOn));
            });
        }
    }
}
