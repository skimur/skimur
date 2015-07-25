using System;
using System.Collections.Generic;
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
    }
}
