using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Sms
{
    public class NoSmsSender : ISmsSender
    {
        ILogger<NoSmsSender> _logger;

        public NoSmsSender(ILogger<NoSmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string number, string message)
        {
            _logger.LogInformation($"Sending '{message}' to '{number}'.");
            return Task.FromResult(0);
        }
    }
}
