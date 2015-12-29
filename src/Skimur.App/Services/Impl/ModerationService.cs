using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Utils;

namespace Skimur.App.Services.Impl
{
    public class ModerationService : IModerationService
    {
        private readonly IDbConnectionProvider _conn;

        public ModerationService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }
        
        public List<Moderator> GetAllModsForSub(Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<Moderator>().Where(x => x.SubId == subId).OrderBy(x => x.AddedOn));
            });
        }

        public void AddModToSub(Guid userId, Guid subId, ModeratorPermissions permissions, Guid? addedBy = null)
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
                    Permissions = permissions
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

        public Dictionary<Guid, ModeratorPermissions> GetSubsModeratoredByUserWithPermissions(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Moderator>();
                query.Where(x => x.UserId == userId);
                query.SelectExpression = "SELECT \"sub_id\", \"permissions\"";
                return conn.Select(query).ToDictionary(x => x.SubId, x => x.Permissions);
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

        public Moderator GetModeratorInfoForUserInSub(Guid userId, Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Single(conn.From<Moderator>().Where(x => x.UserId == userId && x.SubId == subId));
            });
        }

        public void UpdateUserModPermissionForSub(Guid userId, Guid subId, ModeratorPermissions permissions)
        {
            if(permissions.HasFlag(ModeratorPermissions.All))
                permissions = ModeratorPermissions.All; // clear our out any 'non needed' flags.

            _conn.Perform(conn =>
            {
                return conn.Update<Moderator>(new {Permissions = permissions}, x => x.UserId == userId && x.SubId == subId);
            });
        }
    }
}
