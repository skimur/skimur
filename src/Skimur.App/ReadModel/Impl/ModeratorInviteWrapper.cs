using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;
using Membership.ReadModel;
using Membership.Services;

namespace Subs.ReadModel.Impl
{
    public class ModeratorInviteWrapper : IModeratorInviteWrapper
    {
        private readonly IMembershipDao _membershipDao;
        private readonly ISubDao _subDao;
        private readonly IModerationDao _moderationDao;

        public ModeratorInviteWrapper(IMembershipDao membershipDao,
            ISubDao subDao,
            IModerationDao moderationDao)
        {
            _membershipDao = membershipDao;
            _subDao = subDao;
            _moderationDao = moderationDao;
        }

        public List<ModeratorInviteWrapped> Wrap(List<ModeratorInvite> invites, User currentUser = null)
        {
            var items = invites.Select(x => new ModeratorInviteWrapped(x)).ToList();

            var users = _membershipDao.GetUsersByIds(invites.Select(x => x.UserId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subs = _subDao.GetSubsByIds(invites.Select(x => x.SubId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subModeratorInfo = currentUser != null ? subs.Keys.ToDictionary(x => x, x => _moderationDao.GetModeratorInfoForUserInSub(currentUser.Id, x)) : new Dictionary<Guid, Moderator>();

            foreach (var item in items)
            {
                item.Sub = subs[item.ModeratorInvite.SubId];
                item.User = users[item.ModeratorInvite.UserId];

                if (currentUser == null)
                {
                    item.CanRemove = false;
                }
                else
                {
                    if (item.User.Id == currentUser.Id)
                    {
                        // we can remove ourself
                        item.CanRemove = true;
                        // but we can't change our permissions
                        item.CanChangePermissions = false;
                    }
                    else if (currentUser.IsAdmin)
                    {
                        // admins can always remove people and change permissions
                        item.CanRemove = true;
                        item.CanChangePermissions = true;
                    }
                    else
                    {
                        var currentModeratorInfo = subModeratorInfo.ContainsKey(item.Sub.Id)
                            ? subModeratorInfo[item.Sub.Id]
                            : null;
                        if (currentModeratorInfo != null)
                        {
                            // only users with "all" permission can change invite permissions
                            if (currentModeratorInfo.Permissions.HasPermission(ModeratorPermissions.All))
                            {
                                item.CanRemove = true;
                                item.CanChangePermissions = true;
                            }
                            else
                            {
                                item.CanRemove = false;
                                item.CanChangePermissions = false;
                            }
                        }
                        else
                        {
                            // the user is not a moderator in this sub
                            item.CanRemove = false;
                            item.CanChangePermissions = false;
                        }
                    }
                }
            }

            return items;
        }

        public ModeratorInviteWrapped Wrap(ModeratorInvite invite, User currentUser = null)
        {
            return Wrap(new List<ModeratorInvite> { invite }, currentUser)[0];
        }
    }
}
