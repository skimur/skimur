using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Mvc;

namespace Skimur.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

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
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
