using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("Moderators")]
    public class Moderator : IAggregateRoot
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }

        public Guid? AddedBy { get; set; }

        public DateTime AddedOn { get; set; }

        public ModeratorPermissions Permissions { get; set; }
    }
}
