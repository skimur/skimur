using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Skimur.Web.Infrastructure
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        Func<CookieRedirectContext, Task> _old;
        public CustomCookieAuthenticationEvents()
        {
            _old = OnRedirectToLogin;
            OnRedirectToLogin = OnCustomRedirectToLogin;
        }

        public Task OnCustomRedirectToLogin(CookieRedirectContext context)
        {
            var actionContext = context.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();
            if (actionContext.ActionContext == null)
                return _old(context);

            if (actionContext.ActionContext.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is AjaxAttribute))
            {
                // this is an ajax request, return custom JSON telling user that they must be authenticated.
                var serializerSettings = context
                    .HttpContext
                    .RequestServices
                    .GetRequiredService<IOptions<MvcJsonOptions>>()
                    .Value
                    .SerializerSettings;

                context.Response.ContentType = "application/json";

                using (var writer = new HttpResponseStreamWriter(context.Response.Body, Encoding.UTF8))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        jsonWriter.CloseOutput = false;
                        var jsonSerializer = JsonSerializer.Create(serializerSettings);
                        jsonSerializer.Serialize(jsonWriter, new
                        {
                            success = false,
                            error = "You must be signed in."
                        });
                    }
                }

                return Task.FromResult(0);
            }
            else
            {
                // this is a normal request to an endpoint that is secured.
                // do what ASP.NET used to do.
                return _old(context);
            }
        }
    }
}
