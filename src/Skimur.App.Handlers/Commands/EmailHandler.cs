using Skimur.Email;
using Skimur.Messaging.Handling;
using Skimur.Settings;
using Skimur.App.Commands;

namespace Skimur.App.Handlers.Commands
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
