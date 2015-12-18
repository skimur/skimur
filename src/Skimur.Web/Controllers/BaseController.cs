using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Controllers
{
    public class BaseController : Controller
    {
        protected virtual void AddSuccessMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var successMessages = TempData["SuccessMessages"] as List<string>;

            if (successMessages == null)
            {
                successMessages = new List<string>();
                TempData["SuccessMessages"] = successMessages;
            }

            if (!successMessages.Any(x => x.Equals(message)))
                successMessages.Add(message);
        }

        protected virtual void AddErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var errorMessages = TempData["ErrorMessages"] as List<string>;

            if (errorMessages == null)
            {
                errorMessages = new List<string>();
                TempData["ErrorMessages"] = errorMessages;
            }

            if (!errorMessages.Any(x => x.Equals(message)))
                errorMessages.Add(message);
        }

        public string RenderView(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ActionContext.ActionDescriptor.Name;

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                var engine = Resolver.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = engine.FindPartialView(ActionContext, viewName);

                ViewContext viewContext = new ViewContext(ActionContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult CommonJsonResult(bool success, string error = null)
        {
            return Json(new { success, error });
        }

        public ActionResult CommonJsonResult(string error)
        {
            return !string.IsNullOrEmpty(error) ? CommonJsonResult(false, error) : CommonJsonResult(true);
        }

        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
