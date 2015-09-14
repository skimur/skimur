using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Models;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class ModeratorsController : BaseController
    {
        private readonly ISubDao _subDao;
        private readonly ISubModerationDao _subModerationDao;
        private readonly IModeratorWrapper _moderatorWrapper;
        private readonly IUserContext _userContext;

        public ModeratorsController(ISubDao subDao,
            ISubModerationDao subModerationDao,
            IModeratorWrapper moderatorWrapper,
            IUserContext userContext)
        {
            _subDao = subDao;
            _subModerationDao = subModerationDao;
            _moderatorWrapper = moderatorWrapper;
            _userContext = userContext;
        }

        public ActionResult Moderators(string subName)
        {
            if (string.IsNullOrEmpty(subName))
                throw new NotFoundException();

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                throw new NotFoundException();
            
            var model = new ModeratorsViewModel();

            model.Sub = sub;
            model.Moderators = _moderatorWrapper.Wrap(_subModerationDao.GetAllModsForSub(sub.Id), _userContext.CurrentUser);

            return View(model);
        }
    }
}
