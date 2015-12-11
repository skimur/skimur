using Membership.ReadModel;
using Microsoft.AspNet.Mvc;
using Skimur.Settings;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewComponents
{
    public class SideBarViewComponent : ViewComponent
    {
        IUserContext _userContext;
        ISubWrapper _subWrapper;
        ISubDao _subDao;
        IModerationDao _moderationDao;
        IMembershipDao _membershipDao;
        ISubActivityDao _subActivityDao;
        ISettingsProvider<SubSettings> _subSettings;

        public SideBarViewComponent(IUserContext userContext,
            ISubWrapper subWrapper,
            ISubDao subDao,
            IModerationDao moderationDao,
            IMembershipDao membershipDao,
            ISubActivityDao subActivityDao,
            ISettingsProvider<SubSettings> subSettings)
        {
            _userContext = userContext;
            _subWrapper = subWrapper;
            _subDao = subDao;
            _moderationDao = moderationDao;
            _membershipDao = membershipDao;
            _subActivityDao = subActivityDao;
            _subSettings = subSettings;
        }

        public IViewComponentResult Invoke(string subName, Guid? subId, bool showSearch, bool showCreateSub, bool showSubmit)
        {
            var model = new SidebarViewModel();
            model.ShowSearch = showSearch;
            model.ShowCreateSub = showCreateSub;
            model.ShowSubmit = showSubmit;

            var currentUser = _userContext.CurrentUser;

            if (subId.HasValue)
                model.CurrentSub = _subWrapper.Wrap(_subDao.GetSubById(subId.Value), _userContext.CurrentUser);
            else if (!string.IsNullOrEmpty(subName))
                model.CurrentSub = _subWrapper.Wrap(_subDao.GetSubByName(subName), _userContext.CurrentUser);

            if (model.CurrentSub != null)
            {
                if (_userContext.CurrentUser != null)
                    model.Permissions = _moderationDao.GetUserPermissionsForSub(_userContext.CurrentUser, model.CurrentSub.Sub.Id);

                if (!model.IsModerator)
                    // we only show list of mods if the requesting user is not a mod of this sub
                    model.Moderators = _membershipDao.GetUsersByIds(_moderationDao.GetAllModsForSub(model.CurrentSub.Sub.Id).Select(x => x.UserId).ToList());

                // get the number of active users currently viewing this sub.
                // for normal users, this number may be fuzzed (if low enough) for privacy reasons.
                if (_userContext.CurrentUser != null && _userContext.CurrentUser.IsAdmin)
                    model.NumberOfActiveUsers = _subActivityDao.GetActiveNumberOfUsersForSub(model.CurrentSub.Sub.Id);
                else
                {
                    bool wasFuzzed;
                    model.NumberOfActiveUsers = _subActivityDao.GetActiveNumberOfUsersForSubFuzzed(model.CurrentSub.Sub.Id, out wasFuzzed);
                    model.IsNumberOfActiveUsersFuzzed = wasFuzzed;
                }
            }

            if (model.ShowCreateSub)
            {
                if (currentUser != null)
                {
                    var age = Common.CurrentTime() - currentUser.CreatedDate;
                    if (currentUser.IsAdmin || age.TotalDays >= _subSettings.Settings.MinUserAgeCreateSub)
                        model.CanCreateSub = true;
                }
            }

            return View(model);
        }
    }
}
