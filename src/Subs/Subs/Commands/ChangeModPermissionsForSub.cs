using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class ChangeModPermissionsForSub : ICommand
    {
        public string SubName { get; set; }

        public Guid? SubId { get; set; }

        public Guid RequestingUser { get; set; }

        public Guid UserToChange { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }
}
