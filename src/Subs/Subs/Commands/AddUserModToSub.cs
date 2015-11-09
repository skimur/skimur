using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class AddUserModToSub : ICommandReturns<AddUserModToSubResponse>
    {
        public string SubName { get; set; }

        public Guid? SubId { get; set; }

        public Guid RequestingUser { get; set; }

        public Guid UserToAdd { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }

    public class AddUserModToSubResponse
    {
        public string Error { get; set; }
    }
}
