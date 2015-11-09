using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Skimur.Logging;
using static System.String;

namespace Skimur.Web.Mvc
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute is AllowMultiple = true and users might want to override behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SkimurHandleErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private ActionDescriptor _actionDescriptor;
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _actionDescriptor = filterContext.ActionDescriptor;
            base.OnActionExecuting(filterContext);
        }


        public virtual void OnException(ExceptionContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            
            var exception = filterContext.Exception;
            
            var statusCode = new HttpException(null, exception).GetHttpCode();
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            
            if (IsAjax(filterContext))
            {
                // an error occured during an ajax request.
                // normally, we don't handle errors if customerrors is disabled,
                // but in this case, we always want to return a json object
                // with a description of the error. if custom errors is enabled,
                // the error will be generic. Otherwise, the error will be a stack
                // trace (for debugging).
                // If custom errors are disabled, we need to let the normal ASP.NET exception handler
                // execute so that the user can see useful debugging information.

                // only log actual errors. 404/etc, we don't care about
                if (!filterContext.ExceptionHandled && statusCode == 500)
                    // we want to log all 500 errors. 404, etc, we don't care about.
                    Logger.For(filterContext.Controller.GetType()).Error(string.Format("An error occurred executing controller:{0} and action:{1}", controllerName, actionName), exception);

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();

                if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    // custom errors is not enabled, return a generic error.
                    filterContext.Result = new JsonNetResult()
                    {
                        Data = new
                        {
                            success = false,
                            error = "An unknown error has occured"
                        }
                    };
                    if (statusCode == 401)
                        statusCode = 500; // this prevent's ASP middleware from redirecting this ajax request to login page
                    filterContext.HttpContext.Response.StatusCode = statusCode;
                    // Certain versions of IIS will sometimes use their own error page when
                    // they detect a server error. Setting this property indicates that we
                    // want it to try to render ASP.NET MVC's error page instead.
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }
                else
                {
                    // no custom errors (debug), give stack trace.
                    filterContext.Result = new JsonNetResult()
                    {
                        Data = new
                        {
                            success = false,
                            error = exception.Message + " - " + exception.StackTrace
                        }
                    };
                    filterContext.HttpContext.Response.StatusCode = 200;
                }
            }
            else
            {
                if (!filterContext.ExceptionHandled && statusCode == 500)
                    // we want to log all 500 errors. 404, etc, we don't care about.
                    Logger.For(filterContext.Controller.GetType()).Error(string.Format("An error occurred executing controller:{0} and action:{1}", controllerName, actionName), exception);

                if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                    return;

                // an error occured during a normal page request
                if (statusCode == 404)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "HttpStatus/404",
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 404;
                }
                else if (statusCode == 401)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "HttpStatus/401",
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.ClearContent();
                    // someone through an unauthorized result, indicating that the user should know they are unauthorized.
                    // however, in the ASP middleware, if a response code has 401, then it auto redirects to the login page,
                    // even if the user is authenticated. so, we return status code 200 because we want the user to see that
                    // they are in-fact unauthorized to make this request. if we want to hide the fact that they are not
                    // authorized, the implementation should throw a 404 (request not found) sound the hackers can't identify
                    // if invisible/hidden content is available, but not accessible.
                    filterContext.HttpContext.Response.StatusCode = 200;
                }
                else if (statusCode == 500)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "HttpStatus/500",
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                }
                else
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "HttpStatus/Unknown",
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                }

                // Certain versions of IIS will sometimes use their own error page when
                // they detect a server error. Setting this property indicates that we
                // want it to try to render ASP.NET MVC's error page instead.
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        public virtual bool IsAjax(ExceptionContext filterContext)
        {
            if (_actionDescriptor == null) return false;

            if (_actionDescriptor.GetFilterAttributes(true).OfType<AjaxAttribute>().FirstOrDefault() != null)
                return true;
            
            return false;
        }
    }
}
