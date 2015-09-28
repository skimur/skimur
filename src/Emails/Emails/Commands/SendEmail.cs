using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Emails.Commands
{
    public class SendEmail : ICommand
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
