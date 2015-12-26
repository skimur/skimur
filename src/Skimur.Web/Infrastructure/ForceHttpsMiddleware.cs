using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Skimur.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class ForceHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISettingsProvider<WebSettings> _settings;

        public ForceHttpsMiddleware(RequestDelegate next, ISettingsProvider<WebSettings> settings)
        {
            _next = next;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_settings.Settings.ForceHttps)
            {
                await _next(context);
                return;
            }
            
            if(context.Request.Scheme != "https")
            {
                var url = "https://" + context.Request.Host + context.Request.Path + context.Request.QueryString;
                context.Response.Redirect(url);
                return;
            }

            await _next(context);
        }
    }
}
