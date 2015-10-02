using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Settings;

namespace Skimur.Web.Mvc
{
    public class AnnouncementFilterAttribute : ActionFilterAttribute
    {
        private static ISettingsProvider<WebSettings> _webSettings; 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_webSettings == null)
                _webSettings = SkimurContext.Resolve<ISettingsProvider<WebSettings>>();

            filterContext.Controller.ViewBag.Announcement = _webSettings.Settings.Announcement;

            base.OnActionExecuting(filterContext);
        }
    }
}
