using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

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
