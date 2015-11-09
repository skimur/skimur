using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class BanUserFromSub : ICommandReturns<BanUserFromSubResponse>
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public Guid BannedBy { get; set; }

        public Guid? SubId { get; set; }

        public string SubName { get; set; }

        public DateTime DateBanned { get; set; }

        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }

    public class BanUserFromSubResponse
    {
        public string Error { get; set; }
    }
}
