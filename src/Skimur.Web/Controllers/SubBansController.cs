using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Membership;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class SubBansController : BaseController
    {
        private readonly ISubUserBanDao _subUserBanDao;
        private readonly IPermissionDao _permissionDao;
        private readonly ISubDao _subDao;
        private readonly IUserContext _userContext;
        private readonly ISubUserBanWrapper _subUserBanWrapper;
        private readonly ICommandBus _commandBus;

        public SubBansController(ISubUserBanDao subUserBanDao, 
            IPermissionDao permissionDao,
            ISubDao subDao,
            IUserContext userContext,
            ISubUserBanWrapper subUserBanWrapper,
            ICommandBus commandBus)
        {
            _subUserBanDao = subUserBanDao;
            _permissionDao = permissionDao;
            _subDao = subDao;
            _userContext = userContext;
            _subUserBanWrapper = subUserBanWrapper;
            _commandBus = commandBus;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Bans(string subName, string userName, int? pageNumber)
        {
            if (string.IsNullOrEmpty(subName))
                return Redirect(Url.Subs());

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                throw new HttpException(404, "sub not found");

            if (!_permissionDao.CanUserModerateSub(_userContext.CurrentUser.Id, sub.Id))
                throw new HttpException(403, "not allowed to moderate bans");

            if (!pageNumber.HasValue)
                pageNumber = 1;

            var bannedUsers = _subUserBanDao.GetBannedUsersInSub(sub.Id, userName, (pageNumber.Value - 1)*20, 20);

            var model = new BannedUsersFromSub();
            model.Sub = sub;
            model.Users = new PagedList<SubUserBanWrapped>(_subUserBanWrapper.Wrap(bannedUsers), pageNumber.Value, 20, bannedUsers.HasMore);

            return View(model);
        }

        [HttpPost]
        public ActionResult Ban(string subName, BanUserModel model)
        {
            if (string.IsNullOrEmpty(subName))
                return Redirect(Url.Subs());

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                throw new HttpException(404, "sub not found");

            if (!_permissionDao.CanUserModerateSub(_userContext.CurrentUser.Id, sub.Id))
                throw new HttpException(403, "not allowed to moderate bans");

            try
            {
                var response = _commandBus.Send<BanUserFromSub, BanUserFromSubResponse>(new BanUserFromSub
                {
                    UserName = model.UserName,
                    BannedBy = _userContext.CurrentUser.Id,
                    SubId = sub.Id,
                    DateBanned = Common.CurrentTime(),
                    BannedUntil = model.BannedUntil,
                    ReasonPrivate = model.ReasonPrivate,
                    ReasonPublic = model.ReasonPublic
                });

                if (!string.IsNullOrEmpty(response.Error))
                {
                    return Json(new
                    {
                        success = false,
                        error = response.Error
                    });
                }

                return Json(new
                {
                    success = true,
                    error = (string)null
                });
            }
            catch (Exception ex)
            {
                // TODO: log error
                return Json(new
                {
                    success = false,
                    error = "An unexpected error has occured."
                });
            }
        }
    }
}
