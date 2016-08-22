using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toAddress, string subject, string body);

    }
}
