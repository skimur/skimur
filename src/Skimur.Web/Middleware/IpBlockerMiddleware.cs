using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Infrastructure.Settings;
using Microsoft.Owin;

namespace Skimur.Web.Middleware
{
    public class IpBlockerMiddleware : OwinMiddleware
    {
        private readonly ISettingsProvider<BlockedIps> _settings;

        public IpBlockerMiddleware(OwinMiddleware next, ISettingsProvider<BlockedIps> settings)
            :base(next)
        {
            _settings = settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (_settings.Settings.Ips == null || _settings.Settings.Ips.Count == 0)
            {
                await Next.Invoke(context);
                return;
            }

            var ip = context.Request.RemoteIpAddress;

            if (!string.IsNullOrEmpty(ip) && _settings.Settings.Ips.Contains(ip))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await Next.Invoke(context);
        }
    }
}
