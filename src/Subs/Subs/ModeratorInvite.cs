using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("ModeratorInvites")]
    public class ModeratorInvite
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }

        public Guid? InvitedBy { get; set; }

        public DateTime InvitedOn { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }
}
