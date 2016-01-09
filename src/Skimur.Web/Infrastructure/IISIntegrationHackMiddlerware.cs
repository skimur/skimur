using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class IISIntegrationHackMiddlerware
    {
        // https://github.com/aspnet/IISIntegration/issues/51

        private readonly RequestDelegate _next;

        public IISIntegrationHackMiddlerware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var header = context.Request.Headers["X-Forwarded-For"];
                if (header.Count > 0)
                {
                    var forwardFor = header[0];
                    if (!string.IsNullOrEmpty(forwardFor))
                    {
                        if (forwardFor.Contains(":"))
                        {
                            forwardFor = forwardFor.Substring(0, forwardFor.LastIndexOf(":"));
                            context.Request.Headers["X-Forwarded-For"] = forwardFor;
                        }
                    }
                }
            }
            
            await _next(context);
        }
    }
}
