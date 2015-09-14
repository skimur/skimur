using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Subs.Commands;

namespace Subs.Worker
{
    public class ModerationHandler : 
        ICommandHandler<RemoveModFromSub>,
        ICommandHandler<ChangeModPermissionsForSub>
    {
        public void Handle(RemoveModFromSub command)
        {

        }

        public void Handle(ChangeModPermissionsForSub command)
        {

        }
    }
}
