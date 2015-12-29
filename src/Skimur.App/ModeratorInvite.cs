using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
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
