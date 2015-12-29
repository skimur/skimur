using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Skimur.App;
using Skimur.App.ReadModel;

namespace Skimur.Web.ViewComponents
{
    public class StylesViewComponent : ViewComponent
    {
        IUserContext _userContext;
        ISubDao _subDao;
        IMapper _mapper;
        ISubCssDao _subCssDao;

        public StylesViewComponent(IUserContext userContext,
            ISubDao subDao,
            IMapper mapper,
            ISubCssDao subCssDao)
        {
            _userContext = userContext;
            _subDao = subDao;
            _mapper = mapper;
            _subCssDao = subCssDao;
        }

        public IViewComponentResult Invoke(string subName)
        {
            // the user doesn't want to see any styles!
            if (_userContext.CurrentUser != null && !_userContext.CurrentUser.EnableStyles) return Content(string.Empty);

            var sub = _subDao.GetSubByName(subName);
            if (sub == null) return Content(string.Empty);

            var isPreviewing = false;

            var header = Request.Headers.ContainsKey(HeaderNames.SetCookie);

            if (ViewContext.HttpContext.Response.Headers.ContainsKey(HeaderNames.SetCookie))
            {
                var setCookie = ViewContext.HttpContext.Response.Headers[HeaderNames.SetCookie];
                foreach (var value in setCookie)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value.Contains($"preview-{sub.Name.ToLower()}=;"))
                        {
                            isPreviewing = true;
                            break;
                        }
                    }
                }
            }
            else if (Request.Cookies.ContainsKey($"preview-{sub.Name.ToLower()}"))
            {
                isPreviewing = true;
            }

            if (isPreviewing)
            {
                var preview = HttpContext.Session.Get<StylesPreviewModel>($"preview-{sub.Name.ToLower()}");
                if (preview != null)
                {
                    ViewBag.IsPreview = true;
                    ViewBag.PreviewStylesSubName = sub.Name;

                    return View(_mapper.Map<StylesPreviewModel, SubCss>(preview));
                }
            }

            var styles = _subCssDao.GetStylesForSub(sub.Id);
            if (styles == null) return Content(string.Empty);

            ViewBag.IsPreview = false;
            return View(styles);
        }
    }
}
