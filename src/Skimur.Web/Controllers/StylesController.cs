using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Skimur.Web.Mvc;
using Subs;
using Subs.Commands;
using Subs.ReadModel;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    [SkimurAuthorize]
    public class StylesController : BaseController
    {
        private readonly IPermissionDao _permissionDao;
        private readonly IUserContext _userContext;
        private readonly ISubDao _subDao;
        private readonly ISubCssDao _subStylesDao;
        private readonly IMapper _mapper;
        private readonly ICommandBus _commandBus;

        public StylesController(IPermissionDao permissionDao,
            IUserContext userContext,
            ISubDao subDao,
            ISubCssDao subStylesDao,
            IMapper mapper,
            ICommandBus commandBus)
        {
            _permissionDao = permissionDao;
            _userContext = userContext;
            _subDao = subDao;
            _subStylesDao = subStylesDao;
            _mapper = mapper;
            _commandBus = commandBus;
        }

        public ActionResult Edit(string subName)
        {
            var sub = _subDao.GetSubByName(subName);
            if (sub == null) throw new NotFoundException();

            if (!_permissionDao.CanUserManageSubStyles(_userContext.CurrentUser, sub.Id)) throw new UnauthorizedException();

            var styles = _subStylesDao.GetStylesForSub(sub.Id);
            if (styles == null)
            {
                styles = new SubCss();
                styles.CssType = CssType.None;
            }

            var model = new StylesEditModel();
            _mapper.Map(styles, model);
            model.Sub = sub;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string subName, StylesEditModel model)
        {
            var sub = _subDao.GetSubByName(subName);
            if (sub == null) throw new NotFoundException();

            model.Sub = sub;

            if (!ModelState.IsValid)
                return View(model);

            if (!_permissionDao.CanUserManageSubStyles(_userContext.CurrentUser, sub.Id)) throw new UnauthorizedException();

            var response = _commandBus.Send<EditSubStylesCommand, EditSubStylesCommandResponse>(new EditSubStylesCommand
            {
                EditedByUserId = _userContext.CurrentUser.Id,
                SubId = sub.Id,
                CssType = model.CssType,
                Embedded = model.Embedded,
                ExternalCss = model.ExternalCss,
                GitHubCssProjectName = model.GitHubCssProjectName,
                GitHubCssProjectTag = model.GitHubCssProjectTag,
                GitHubLessProjectName = model.GitHubLessProjectName,
                GitHubLessProjectTag = model.GitHubLessProjectTag
            });

            if (string.IsNullOrEmpty(response.Error))
            {
                AddSuccessMessage("The styles have succesfully been updated.");
            }
            else
            {
                AddErrorMessage(response.Error);
            }

            return View(model);
        }

        public ActionResult SubStyles(string subName)
        {
            var sub = _subDao.GetSubByName(subName);
            if (sub == null) return Content("");

            var styles = _subStylesDao.GetStylesForSub(sub.Id);
            if (styles == null) return Content("");

            switch (styles.CssType)
            {
                case CssType.None:
                    return Content("");
                case CssType.Embedded:
                    return Content("<style type=\"text/css\">" + styles.Embedded + "</style>");
                case CssType.ExternalCss:
                    return !string.IsNullOrEmpty(styles.ExternalCss) ? Content("<link href=\"" + styles.ExternalCss + "\" rel=\"stylesheet\" />") : Content("");
                case CssType.GitHubCss:
                    // TODO: Serve up github content ourselves to avoid 3rd party dependency of "rawgit.com".
                    if (!string.IsNullOrEmpty(styles.GitHubCssProjectName) &&
                        !string.IsNullOrEmpty(styles.GitHubCssProjectTag))
                        return Content("<link href=\"https://cdn.rawgit.com/" + styles.GitHubCssProjectName  + "/" + styles.GitHubCssProjectTag + "/file\" rel=\"stylesheet\" />");
                    return Content("");
                case CssType.GitHubLess:
                    return Content(""); // not supported yet.
                default:
                    return Content("");
            }
        }
    }
}
