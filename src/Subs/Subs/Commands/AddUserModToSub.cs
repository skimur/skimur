using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

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
