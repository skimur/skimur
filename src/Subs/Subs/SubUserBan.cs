using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("SubUserBans")]
    public class SubUserBan : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid SubId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? BannedUntil { get; set; }

        public DateTime DateBanned { get; set; }

        public Guid BannedBy { get; set; }

        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }
}
