using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class RemoveModInviteFromSub : ICommandReturns<RemoveModInviteFromSubResponse>
    {
        public Guid RequestingUserId { get; set; }

        public string UserName { get; set; }

        public Guid? UserId { get; set; }

        public string SubName { get; set; }

        public Guid? SubId { get; set; }
    }

    public class RemoveModInviteFromSubResponse
    {
        public string Error { get; set; }
    }
}
