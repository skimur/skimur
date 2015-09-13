using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Membership.Services;
using Skimur.Web.Models;
using Subs.ReadModel;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly ISubModerationDao _subModerationDao;

        public UsersController(IMembershipService membershipService,
            ISubDao subDao,
            ISubModerationDao subModerationDao)
        {
            _membershipService = membershipService;
            _subDao = subDao;
            _subModerationDao = subModerationDao;
        }

        public ActionResult User(string userName)
        {
            var user = _membershipService.GetUserByUserName(userName);

            if(user == null)
                throw new NotFoundException();
            
            var model = new UserViewModel();
            model.User = user;
            
            var moderatedSubs = _subModerationDao.GetSubsModeratoredByUser(user.Id);

            if (moderatedSubs.Count > 0)
            {
                model.IsModerator = true;
                model.ModeratingSubs = _subDao.GetSubsByIds(moderatedSubs).Select(x => x.Name).ToList();
            }

            return View(model);
        }
    }
}
