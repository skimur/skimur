using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Subs;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class ModeratorsController : BaseController
    {
        private readonly ISubDao _subDao;
        private readonly ISubModerationDao _subModerationDao;
        private readonly IModeratorWrapper _moderatorWrapper;
        private readonly IUserContext _userContext;
        private readonly ICommandBus _commandBus;
        private readonly ILogger<ModeratorsController> _logger;

        public ModeratorsController(ISubDao subDao,
            ISubModerationDao subModerationDao,
            IModeratorWrapper moderatorWrapper,
            IUserContext userContext,
            ICommandBus commandBus,
            ILogger<ModeratorsController> logger)
        {
            _subDao = subDao;
            _subModerationDao = subModerationDao;
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
            model.Moderators = _moderatorWrapper.Wrap(_subModerationDao.GetAllModsForSub(sub.Id), _userContext.CurrentUser);

            return View(model);
        }

        public ActionResult RemoveModFromSub(string subName, Guid? subId, Guid userId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to remove a mod."
                });
            }
            
            try
            {
                _commandBus.Send(new RemoveModFromSub
                {
                    SubName = subName,
                    SubId = subId,
                    RequestingUser = _userContext.CurrentUser.Id,
                    UserToRemove = userId
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error removing mod from sub.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }

        public ActionResult ChangeModPermissions(string subName, Guid? subId, Guid userId, ModeratorPermissions permissions)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to remove a mod."
                });
            }

            try
            {
                _commandBus.Send(new ChangeModPermissionsForSub
                {
                    SubName = subName,
                    SubId = subId,
                    RequestingUser = _userContext.CurrentUser.Id,
                    UserToChange = userId,
                    Permissions = permissions
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error changing permissions for mod on sub.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }
    }
}
