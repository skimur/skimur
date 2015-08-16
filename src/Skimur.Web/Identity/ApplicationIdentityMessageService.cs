using System.Threading.Tasks;
using Infrastructure.Email;
using Microsoft.AspNet.Identity;

namespace Skimur.Web.Identity
{
    public class ApplicationIdentityMessageService : IIdentityMessageService
    {
        private readonly IEmailSender _emailSender;
        private readonly IQueuedEmailService _queuedEmailService;

        public ApplicationIdentityMessageService(IEmailSender emailSender, IQueuedEmailService queuedEmailService)
        {
            _emailSender = emailSender;
            _queuedEmailService = queuedEmailService;
        }

        public Task SendAsync(IdentityMessage message)
        {
            var queuedEmail = new QueuedEmail()
            {
                Subject = message.Subject,
                Body = message.Body,
                From = "noreply@email.com",
                FromName = "noreply",
                To = message.Destination
            };
            return Task.Factory.StartNew(() => _queuedEmailService.InsertQueuedEmail(queuedEmail));
        }
    }
}
