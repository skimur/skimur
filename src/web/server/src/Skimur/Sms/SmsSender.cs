using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Sms
{
    public class NoSmsSender : ISmsSender
    {
        public Task SendSmsAsync(string number, string message)
        {
            throw new NotSupportedException();
        }
    }
}
