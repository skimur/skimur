using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Sms
{
    public class NoSmsSender : ISmsSender
    {
        ILogger _logger;

        public NoSmsSender(ILogger<ISmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string number, string message)
        {
            _logger.LogInformation($"Sending '{message}' to '{number}'.");
            Console.WriteLine($"Sending '{message}' to '{number}'.");
            return Task.FromResult(0);
        }
    }
}
