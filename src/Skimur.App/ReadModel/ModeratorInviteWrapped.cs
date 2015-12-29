using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.ReadModel
{
    public class ModeratorInviteWrapped
    {
        public ModeratorInviteWrapped(ModeratorInvite moderatorInvite)
        {
            ModeratorInvite = moderatorInvite;
        }

        public ModeratorInvite ModeratorInvite { get; set; }

        public Sub Sub { get; set; }

        public User User { get; set; }

        public bool CanRemove { get; set; }

        public bool CanChangePermissions { get; set; }
    }
}
