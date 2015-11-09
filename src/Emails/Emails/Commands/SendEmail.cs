using Skimur.Messaging;

namespace Emails.Commands
{
    public class SendEmail : ICommand
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
