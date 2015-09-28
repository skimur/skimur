using System.Threading.Tasks;
using Emails.Commands;
using Infrastructure.Email;
using Infrastructure.Messaging;
using Microsoft.AspNet.Identity;

namespace Skimur.Web.Identity
{
    public class ApplicationIdentityMessageService : IIdentityMessageService
    {
        private readonly ICommandBus _commandBus;

        public ApplicationIdentityMessageService(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public Task SendAsync(IdentityMessage message)
        {
            _commandBus.Send(new SendEmail
            {
                Email = message.Destination,
                Subject = message.Subject,
                Body = message.Body
            });
            return Task.FromResult(0);
        }
    }
}
