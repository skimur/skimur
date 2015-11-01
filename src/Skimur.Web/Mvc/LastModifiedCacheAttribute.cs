using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Skimur.Web.Mvc
{
    public class LastModifiedCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is FilePathResult)
            {
                var result = (FilePathResult)filterContext.Result;
                var lastModify = File.GetLastWriteTime(result.FileName);
                if (!HasModification(filterContext.RequestContext, lastModify))
                    filterContext.Result = NotModified(filterContext.RequestContext, lastModify);
                SetLastModifiedDate(filterContext.RequestContext, lastModify);
            }
            base.OnActionExecuted(filterContext);
        }

        private static void SetLastModifiedDate(RequestContext requestContext, DateTime modificationDate)
        {
            requestContext.HttpContext.Response.Cache.SetLastModified(modificationDate);
        }

        private static bool HasModification(RequestContext context, DateTime modificationDate)
        {
            var headerValue = context.HttpContext.Request.Headers["If-Modified-Since"];
            if (headerValue == null)
                return true;
            var modifiedSince = DateTime.Parse(headerValue).ToLocalTime();
            return modifiedSince < modificationDate;
        }

        private static ActionResult NotModified(RequestContext response, DateTime lastModificationDate)
        {
            response.HttpContext.Response.Cache.SetLastModified(lastModificationDate);
            return new HttpStatusCodeResult(304, "Page has not been modified");
        }
    }
}
