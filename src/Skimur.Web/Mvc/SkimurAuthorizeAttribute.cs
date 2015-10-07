using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web.Mvc
{
    public class SkimurAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (IsAjax(filterContext))
            {
                filterContext.Result = new JsonNetResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        success = false,
                        error = "You must be signed in."
                    }
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

        private bool IsAjax(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.GetFilterAttributes(true).OfType<AjaxAttribute>().FirstOrDefault() !=
                   null;
        }
    }
}
