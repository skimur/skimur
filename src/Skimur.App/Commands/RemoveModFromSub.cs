using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class RemoveModFromSub : ICommandReturns<RemoveModFromSubResponse>
    {
        public string SubName { get; set; }

        public Guid? SubId { get; set; }

        public Guid RequestingUser { get; set; }

        public string UserNameToRemove { get; set; }

        public Guid? UserIdToRemove { get; set; }
    }

    public class RemoveModFromSubResponse
    {
        public string Error { get; set; }
    }
}
