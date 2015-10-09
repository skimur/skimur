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
        private readonly IModerationDao _moderationDao;
        private readonly IKarmaDao _karmaDao;

        public UsersController(IMembershipService membershipService,
            ISubDao subDao,
            IModerationDao moderationDao,
            IKarmaDao karmaDao)
        {
            _membershipService = membershipService;
            _subDao = subDao;
            _moderationDao = moderationDao;
            _karmaDao = karmaDao;
        }

        public ActionResult User(string userName)
        {
            var user = _membershipService.GetUserByUserName(userName);

            if(user == null)
                throw new NotFoundException();
            
            var model = new UserViewModel();
            model.User = user;
            
            var moderatedSubs = _moderationDao.GetSubsModeratoredByUser(user.Id);

            if (moderatedSubs.Count > 0)
            {
                model.IsModerator = true;
                model.ModeratingSubs = _subDao.GetSubsByIds(moderatedSubs).Select(x => x.Name).ToList();
            }

            var kudos = _karmaDao.GetKarma(user.Id);

            model.CommentKudos = kudos.Keys.Where(x => x.Type == KarmaType.Comment).Sum(x => kudos[x]);
            model.PostKudos = kudos.Keys.Where(x => x.Type == KarmaType.Post).Sum(x => kudos[x]);

            return View(model);
        }
    }
}
