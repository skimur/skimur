using Skimur.App.Commands;
using Skimur.Messaging;
using System.Threading.Tasks;

namespace Skimur.Web.Services.Impl
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        ICommandBus _commandBus;

        public AuthMessageSender(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            _commandBus.Send(new SendEmail
            {
                Email = email,
                Subject = subject,
                Body = message
            });
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
