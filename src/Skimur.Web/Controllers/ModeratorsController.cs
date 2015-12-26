using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Skimur.Logging;
using Skimur.Messaging;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs;
using Subs.Commands;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Controllers
{
    public class ModeratorsController : BaseController
    {
        private readonly ISubDao _subDao;
        private readonly IModerationDao _moderationDao;
        private readonly IModeratorWrapper _moderatorWrapper;
        private readonly IUserContext _userContext;
        private readonly ICommandBus _commandBus;
        private readonly IModeratorInviteWrapper _moderatorInviteWrapper;
        private readonly IPermissionDao _permissionDao;
        private readonly IModerationInviteDao _moderationInviteDao;

        public ModeratorsController(ISubDao subDao,
            IModerationDao moderationDao,
            IModeratorWrapper moderatorWrapper,
            IUserContext userContext,
            ICommandBus commandBus,
            ILogger<ModeratorsController> logger,
            IModeratorInviteWrapper moderatorInviteWrapper,
            IPermissionDao permissionDao,
            IModerationInviteDao moderationInviteDao)
        {
            _subDao = subDao;
            _moderationDao = moderationDao;
            _moderatorWrapper = moderatorWrapper;
            _userContext = userContext;
            _commandBus = commandBus;
            _moderatorInviteWrapper = moderatorInviteWrapper;
            _permissionDao = permissionDao;
            _moderationInviteDao = moderationInviteDao;
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
            if (_userContext.CurrentUser != null)
            {
                if (_permissionDao.GetUserPermissionsForSub(_userContext.CurrentUser, sub.Id).HasPermission(ModeratorPermissions.All))
                {
                    // super mods can see all pending invites
                    model.Invites = _moderatorInviteWrapper.Wrap(_moderationInviteDao.GetModeratorInvitesForSub(sub.Id));
                    model.CanInvite = true;
                }
                else
                {
                    // let's see if the current user has a pending invite
                    model.CurrentUserInvite = _moderationInviteDao.GetModeratorInviteInfo(_userContext.CurrentUser.Id, sub.Id);
                }
            }

            return View(model);
        }

        [Authorize, Ajax]
        public ActionResult RemoveModFromSub(string subName, Guid? subId, Guid? userId, string userName)
        {
            var response = _commandBus.Send<RemoveModFromSub, RemoveModFromSubResponse>(new RemoveModFromSub
            {
                SubName = subName,
                SubId = subId,
                RequestingUser = _userContext.CurrentUser.Id,
                UserNameToRemove = userName,
                UserIdToRemove = userId
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult ChangeModPermissions(string subName, Guid? subId, string userName, Guid? userId, ModeratorPermissions permissions)
        {
            var response = _commandBus.Send<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>(new ChangeModPermissionsForSub
            {
                SubName = subName,
                SubId = subId,
                RequestingUser = _userContext.CurrentUser.Id,
                UserNameToChange = userName,
                UserIdToChange = userId,
                Permissions = permissions
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult InviteMod(string subName, Guid? subId, string userName, Guid? userId, ModeratorPermissions permissions)
        {
            var response = _commandBus.Send<InviteModToSub, InviteModToSubResponse>(new InviteModToSub
            {
                RequestingUserId = _userContext.CurrentUser.Id,
                UserName = userName,
                UserId = userId,
                SubName = subName,
                SubId = subId,
                Permissions = permissions
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult AcceptInvite(string subName, Guid? subId)
        {
            var response = _commandBus.Send<AcceptModInvitation, AcceptModInvitationResponse>(new AcceptModInvitation
            {
                UserId = _userContext.CurrentUser.Id,
                SubName = subName,
                SubId = subId
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult DenyInvite(string subName, Guid? subId)
        {
            var response = _commandBus.Send<RemoveModInviteFromSub, RemoveModInviteFromSubResponse>(new RemoveModInviteFromSub
            {
                RequestingUserId = _userContext.CurrentUser.Id,
                UserId = _userContext.CurrentUser.Id,
                SubName = subName,
                SubId = subId
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult RemoveInvite(string subName, Guid? subId, string userName, Guid? userId)
        {
            var response = _commandBus.Send<RemoveModInviteFromSub, RemoveModInviteFromSubResponse>(new RemoveModInviteFromSub
            {
                RequestingUserId = _userContext.CurrentUser.Id,
                UserName = userName,
                UserId = userId,
                SubName = subName,
                SubId = subId
            });

            return CommonJsonResult(response.Error);
        }

        [Authorize, Ajax]
        public ActionResult ChangeInvite(string subName, Guid? subId, string userName, Guid? userId, ModeratorPermissions permissions)
        {
            var response = _commandBus.Send<ChangeModInvitePermissions, ChangeModInvitePermissionsResponse>(new ChangeModInvitePermissions
            {
                RequestingUserId = _userContext.CurrentUser.Id,
                UserName = userName,
                UserId = userId,
                SubName = subName,
                SubId = subId,
                Permissions = permissions
            });

            return CommonJsonResult(response.Error);
        }
    }
}
