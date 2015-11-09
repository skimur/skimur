using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Skimur.Logging;
using Skimur.Settings;

namespace Skimur.Web.Middleware
{
    public class RequestLoggingMiddleware : OwinMiddleware
    {
        private readonly ISettingsProvider<RequestLogging> _settings;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(OwinMiddleware next, ISettingsProvider<RequestLogging> settings, ILogger<RequestLoggingMiddleware> logger)
            :base(next)
        {
            _settings = settings;
            _logger = logger;
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            if (!_settings.Settings.Enabled) return;

            if (_settings.Settings.ExcludeStatusCodes != null && _settings.Settings.ExcludeStatusCodes.Count > 0)
            {
                if (_settings.Settings.ExcludeStatusCodes.Contains(context.Response.StatusCode))
                    return;
            }
            
            _logger.Info("ip:" + context.Request.RemoteIpAddress + ":url:" + context.Request.Uri);
        }
    }
}
