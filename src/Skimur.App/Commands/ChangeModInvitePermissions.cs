using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class ChangeModInvitePermissions : ICommandReturns<ChangeModInvitePermissionsResponse>
    {
        public Guid RequestingUserId { get; set; }

        public string UserName { get; set; }

        public Guid? UserId { get; set; }

        public string SubName { get; set; }

        public Guid? SubId { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }

    public class ChangeModInvitePermissionsResponse
    {
        public string Error { get; set; }
    }
}
