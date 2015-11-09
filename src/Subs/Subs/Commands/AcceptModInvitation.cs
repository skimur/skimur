using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class AcceptModInvitation : ICommandReturns<AcceptModInvitationResponse>
    {
        public string UserName { get; set; }

        public Guid? UserId { get; set; }

        public string SubName { get; set; }

        public Guid? SubId { get; set; }
    }

    public class AcceptModInvitationResponse
    {
        public string Error { get; set; }
    }
}
