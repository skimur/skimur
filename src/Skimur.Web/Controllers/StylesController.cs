﻿using Microsoft.AspNet.Mvc;
using Skimur.Messaging;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Skimur.Web.Infrastructure;
using Microsoft.AspNet.Authorization;
using Skimur.App;
using Skimur.App.Commands;
using Skimur.App.ReadModel;

namespace Skimur.Web.Controllers
{
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

        [Authorize]
        public ActionResult Edit(string subName)
        {
            var sub = _subDao.GetSubByName(subName);
            if (sub == null) throw new NotFoundException();

            if (!_permissionDao.CanUserManageSubStyles(_userContext.CurrentUser, sub.Id)) throw new UnauthorizedException();

            var model = new StylesEditModel();
            model.Sub = sub;
            model.StyledEnabledForUser = _userContext.CurrentUser.EnableStyles;
            
            if (Request.Cookies.ContainsKey("preview-" + sub.Name.ToLower()))
            {
                var preview = HttpContext.Session.Get<StylesPreviewModel>("preview-" + sub.Name.ToLower());
                if (preview != null)
                {
                    _mapper.Map(preview, model);
                    return View(model);
                }
            }

            var styles = _subStylesDao.GetStylesForSub(sub.Id);
            if (styles == null)
            {
                styles = new SubCss();
                styles.CssType = CssType.None;
            }

            _mapper.Map(styles, model);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ParameterBasedOnFormName("save", "shouldSave"), ParameterBasedOnFormName("preview", "shouldPreview")]
        public ActionResult Edit(string subName, StylesEditModel model, bool shouldSave, bool shouldPreview)
        {
            var sub = _subDao.GetSubByName(subName);
            if (sub == null) throw new NotFoundException();

            model.Sub = sub;
            model.StyledEnabledForUser = _userContext.CurrentUser.EnableStyles;

            if (!ModelState.IsValid)
                return View(model);

            if (!_permissionDao.CanUserManageSubStyles(_userContext.CurrentUser, sub.Id)) throw new UnauthorizedException();

            if (shouldSave)
            {
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
                    CancelPreview(sub.Name, string.Empty); // this will clearly any preview session that may be going on
                }
                else
                {
                    AddErrorMessage(response.Error);
                }
            }
            else if (shouldPreview)
            {
                HttpContext.Session.Set("preview-" + sub.Name.ToLower(), _mapper.Map<StylesEditModel, StylesPreviewModel>(model));
                Response.Cookies.Append("preview-" + sub.Name.ToLower(), string.Empty);
            }

            return View(model);
        }

        public ActionResult CancelPreview(string subName, string returnUrl)
        {
            if (!string.IsNullOrEmpty(subName))
            {
                subName = subName.ToLower();
                HttpContext.Session.Set<StylesPreviewModel>("preview-" + subName.ToLower(), null);
                Response.Cookies.Delete("preview-" + subName.ToLower());
            }

            return RedirectToLocal(returnUrl);
        }

        //[ChildActionOnly]
        //public ActionResult SubStyles(string subName)
        //{
        //    // the user doesn't want to see any styles!
        //    if (_userContext.CurrentUser != null && !_userContext.CurrentUser.EnableStyles) return Content("");

        //    var sub = _subDao.GetSubByName(subName);
        //    if (sub == null) return Content("");

        //    if (Request.Cookies.AllKeys.Contains("preview-" + sub.Name.ToLower()))
        //    {
        //        HttpContext.Session.GetInt("");
        //        var preview = HttpContext.Session["preview-" + sub.Name.ToLower()] as StylesPreviewModel;
        //        if (preview != null)
        //        {
        //            ViewBag.IsPreview = true;
        //            ViewBag.PreviewStylesSubName = sub.Name;
        //            return PartialView("RenderStyles", _mapper.Map<StylesPreviewModel, SubCss>(preview));
        //        }
        //    }

        //    var styles = _subStylesDao.GetStylesForSub(sub.Id);
        //    if (styles == null) return Content("");

        //    ViewBag.IsPreview = false;
        //    return PartialView("RenderStyles", styles);
        //}
    }
}
