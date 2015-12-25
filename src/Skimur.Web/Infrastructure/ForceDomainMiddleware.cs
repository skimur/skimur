using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Skimur.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class ForceDomainMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISettingsProvider<WebSettings> _settings;

        public ForceDomainMiddleware(RequestDelegate next, ISettingsProvider<WebSettings> settings)
        {
            _next = next;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.IsNullOrEmpty(_settings.Settings.ForceDomain))
            {
                await _next(context);
                return;
            }

            // concurrency, in case the setting is updated live
            var domain = _settings.Settings.ForceDomain;
            if (string.IsNullOrEmpty(domain))
            {
                await _next(context);
                return;
            }
            
            if(context.Request.Host.HasValue)
            {
                if(!string.Equals(context.Request.Host.Value, domain, StringComparison.OrdinalIgnoreCase))
                {
                    var url = "https://" + domain + context.Request.Path + context.Request.QueryString;
                    context.Response.Redirect(url);
                    return;
                }
            }
            
            await _next(context);
        }
    }
}
