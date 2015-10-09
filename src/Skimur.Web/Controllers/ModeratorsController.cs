using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Skimur.Web.Mvc;
using Subs;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class ModeratorsController : BaseController
    {
        private readonly ISubDao _subDao;
        private readonly IModerationDao _moderationDao;
        private readonly IModeratorWrapper _moderatorWrapper;
        private readonly IUserContext _userContext;
        private readonly ICommandBus _commandBus;
        private readonly ILogger<ModeratorsController> _logger;

        public ModeratorsController(ISubDao subDao,
            IModerationDao moderationDao,
            IModeratorWrapper moderatorWrapper,
            IUserContext userContext,
            ICommandBus commandBus,
            ILogger<ModeratorsController> logger)
        {
            _subDao = subDao;
            _moderationDao = moderationDao;
            _moderatorWrapper = moderatorWrapper;
            _userContext = userContext;
            _commandBus = commandBus;
            _logger = logger;
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
            model.Moderators = _moderatorWrapper.Wrap(_moderationDao.GetAllModsForSub(sub.Id), _userContext.CurrentUser);

            return View(model);
        }

        [SkimurAuthorize, Ajax]
        public ActionResult RemoveModFromSub(string subName, Guid? subId, Guid userId)
        {
            var response = _commandBus.Send<RemoveModFromSub, RemoveModFromSubResponse>(new RemoveModFromSub
            {
                SubName = subName,
                SubId = subId,
                RequestingUser = _userContext.CurrentUser.Id,
                UserToRemove = userId
            });

            return CommonJsonResult(response.Error);
        }

        [SkimurAuthorize, Ajax]
        public ActionResult ChangeModPermissions(string subName, Guid? subId, Guid userId, ModeratorPermissions permissions)
        {
            var response = _commandBus.Send<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>(new ChangeModPermissionsForSub
            {
                SubName = subName,
                SubId = subId,
                RequestingUser = _userContext.CurrentUser.Id,
                UserToChange = userId,
                Permissions = permissions
            });

            return CommonJsonResult(response.Error);
        }
    }
}
