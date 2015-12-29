using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
{
    [Alias("SubUserBans")]
    public class SubUserBan
    {
        public Guid Id { get; set; }

        public Guid SubId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public DateTime DateBanned { get; set; }

        public Guid BannedBy { get; set; }

        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }
}
