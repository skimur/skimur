using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;
using Membership.Services;

namespace Subs.ReadModel.Impl
{
    public class ModeratorWrapper : IModeratorWrapper
    {
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IPermissionDao _permissionDao;
        private readonly IModerationDao _moderationDao;

        public ModeratorWrapper(IMembershipService membershipService,
            ISubDao subDao,
            IPermissionDao permissionDao,
            IModerationDao moderationDao)
        {
            _membershipService = membershipService;
            _subDao = subDao;
            _permissionDao = permissionDao;
            _moderationDao = moderationDao;
        }

        public List<ModeratorWrapped> Wrap(List<Moderator> moderators, User currentUser = null)
        {
            var items = moderators.Select(x => new ModeratorWrapped(x)).ToList();

            var users = _membershipService.GetUsersByIds(moderators.Select(x => x.UserId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subs = _subDao.GetSubsByIds(moderators.Select(x => x.SubId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subModeratorInfo = currentUser != null ? subs.Keys.ToDictionary(x => x, x => _moderationDao.GetModeratorInfoForUserInSub(currentUser.Id, x)) : new Dictionary<Guid, Moderator>();
            
            foreach (var item in items)
            {
                item.Sub = subs[item.Moderator.SubId];
                item.User = users[item.Moderator.UserId];

                if (currentUser == null)
                {
                    item.CanRemove = false;
                    item.CanChangePermissions = false;
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
                            // the user is a moderator of some kind in the sub
                            // remember, a mod can never remove or change permissions of a mod who has been one longer
                            if (currentModeratorInfo.AddedOn > item.Moderator.AddedOn)
                            {
                                // the current user was added as a moderator later than this current interated one.
                                // seniority!
                                item.CanRemove = false;
                                item.CanChangePermissions = false;
                            }
                            else
                            {
                                // only users with "all" permission can change/remove another mod
                                if(currentModeratorInfo.Permissions.HasPermission(ModeratorPermissions.All))
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

        public ModeratorWrapped Wrap(Moderator moderator, User currentUser = null)
        {
            return Wrap(new List<Moderator> {moderator}, currentUser)[0];
        }
    }
}
