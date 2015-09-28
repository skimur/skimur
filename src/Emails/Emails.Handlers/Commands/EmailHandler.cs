using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emails.Commands;
using Infrastructure.Email;
using Infrastructure.Messaging.Handling;
using Infrastructure.Settings;

namespace Emails.Handlers.Commands
{
    public class EmailHandler : ICommandHandler<SendEmail>
    {
        private readonly IEmailSender _emailSender;
        private readonly ISettingsProvider<EmailServerSettings> _emailSettings;

        public EmailHandler(IEmailSender emailSender, ISettingsProvider<EmailServerSettings> emailSettings)
        {
            _emailSender = emailSender;
            _emailSettings = emailSettings;
        }

        public void Handle(SendEmail command)
        {
            _emailSender.SendEmail(_emailSettings.Settings, command.Subject, command.Body, null, null, command.Email, null);
        }
    }
}
