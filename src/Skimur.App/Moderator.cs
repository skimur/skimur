using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
{
    [Alias("Moderators")]
    public class Moderator
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }

        public Guid? AddedBy { get; set; }

        public DateTime AddedOn { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }
}
