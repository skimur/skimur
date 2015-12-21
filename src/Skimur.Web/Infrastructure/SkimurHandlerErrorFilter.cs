using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Hosting;

namespace Skimur.Web.Infrastructure
{
    public class SkimurHandlerErrorFilter : ActionFilterAttribute, IExceptionFilter
    {
        private ActionDescriptor _actionDescriptor;
        private object _controller;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _controller = filterContext.Controller;
            _actionDescriptor = filterContext.ActionDescriptor;
            base.OnActionExecuting(filterContext);
        }

        public virtual void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            var serviceProvider = filterContext.HttpContext.Features.Get<IServiceProvidersFeature>();
            var hostingEnvironment = serviceProvider.RequestServices.GetRequiredService<IHostingEnvironment>();

            if (!(exception is BaseHttpException))
            {
                serviceProvider.RequestServices.GetService<ILoggerFactory>().CreateLogger(filterContext.ActionDescriptor.DisplayName)
                    .LogError($"An error occured executing action {filterContext.ActionDescriptor.Name}.", exception);
            }

            if (IsAjax())
            {
                // an error occured during an ajax request.
                // normally, we don't handle errors if customerrors is disabled,
                // but in this case, we always want to return a json object
                // with a description of the error. if custom errors is enabled,
                // the error will be generic. Otherwise, the error will be a stack
                // trace (for debugging).
                // If custom errors are disabled, we need to let the normal ASP.NET exception handler
                // execute so that the user can see useful debugging information.
                
                string errorMessage;
                if (hostingEnvironment.IsDevelopment())
                {
                    errorMessage = exception.Message + " - " + exception.StackTrace;
                }
                else
                {
                    errorMessage = "An unknown error has occured";
                }
                
                filterContext.Result = new JsonResult(new
                {
                    success = false,
                    error = errorMessage
                });

                // we always want a valid response code for ajax requests.
                filterContext.HttpContext.Response.StatusCode = 200;
            }
            else
            {
                if (exception is BaseHttpException)
                {
                    // this is a standard exception thrown for a safe reason
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "HttpStatus/Common",
                        StatusCode = (int)((BaseHttpException)exception).StatusCode
                    };
                }
                else
                {
                    // this is an real exception that needs to be handled/logged.
                    if(hostingEnvironment.IsDevelopment())
                    {
                        // don't do anything with the request, let standard middle ware handle the error page (for debugging).
                    }
                    else
                    {
                        // we are in production environment. show a friendly error message.
                        filterContext.Result = new ViewResult
                        {
                            ViewName = "HttpStatus/Common",
                            StatusCode = 500
                        };
                    }
                }
            }
        }

        protected bool IsAjax()
        {
            if (_actionDescriptor == null) return false;

            return _actionDescriptor.FilterDescriptors.Any(x => x.Filter is AjaxAttribute);
        }
    }
}
