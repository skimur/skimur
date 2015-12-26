using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Skimur.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class IpBlockerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISettingsProvider<BlockedIps> _settings;

        public IpBlockerMiddleware(RequestDelegate next, ISettingsProvider<BlockedIps> settings)
        {
            _next = next;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_settings.Settings.Ips == null || _settings.Settings.Ips.Count == 0)
            {
                await _next(context);
                return;
            }
            
            var remoteIpAddress = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;

            var ip = remoteIpAddress != null ? remoteIpAddress.ToString() : "";

            if (!string.IsNullOrEmpty(ip) && _settings.Settings.Ips.Contains(ip))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await _next(context);
        }
    }
}
