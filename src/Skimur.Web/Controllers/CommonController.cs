using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Settings;
using Membership.Services;
using Skimur.Web.Models;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IContextService _contextService;
        private readonly ISubDao _subDao;
        private readonly IUserContext _userContext;
        private readonly IMessageDao _messageDao;
        private readonly ISubWrapper _subWrapper;
        private readonly IMembershipService _membershipService;
        private readonly ISubActivityDao _subActivityDao;
        private readonly ISettingsProvider<SubSettings> _subSettings;
        private readonly ISubModerationDao _subModerationDao;

        public CommonController(IContextService contextService,
            ISubDao subDao,
            IUserContext userContext,
            IMessageDao messageDao,
            ISubWrapper subWrapper,
            IMembershipService membershipService,
            ISubActivityDao subActivityDao,
            ISettingsProvider<SubSettings> subSettings,
            ISubModerationDao subModerationDao)
        {
            _contextService = contextService;
            _subDao = subDao;
            _userContext = userContext;
            _messageDao = messageDao;
            _subWrapper = subWrapper;
            _membershipService = membershipService;
            _subActivityDao = subActivityDao;
            _subSettings = subSettings;
            _subModerationDao = subModerationDao;
        }
        
        public ActionResult TopBar()
        {
            var model = new TopBarViewModel();
            model.SubscibedSubs.AddRange(_subDao.GetSubsByIds(_contextService.GetSubscribedSubIds()).Select(x => x.Name).OrderBy(x => x));
            model.DefaultSubs.AddRange(_subDao.GetSubsByIds(_subDao.GetDefaultSubs()).Select(x => x.Name));
            return PartialView(model); 
        }

        public ActionResult Account()
        {
            var model = new AccountViewModel();
            model.CurrentUser = _userContext.CurrentUser;
            if (model.CurrentUser != null)
                model.NumberOfUnreadMessages = _messageDao.GetNumberOfUnreadMessagesForUser(model.CurrentUser.Id);
            return PartialView(model);
        }

        public ActionResult SideBar(string subName, Guid? subId, bool showSearch = true, bool showCreateSub = true, bool showSubmit = true)
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
                    model.Permissions = _subModerationDao.GetUserPermissionsForSub(_userContext.CurrentUser, model.CurrentSub.Sub.Id);

                if (!model.IsModerator)
                    // we only show list of mods if the requesting user is not a mod of this sub
                    model.Moderators = _membershipService.GetUsersByIds(_subModerationDao.GetAllModsForSub(model.CurrentSub.Sub.Id).Select(x => x.UserId).ToList());

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

            return PartialView("SideBar", model);
        }
    }
}
