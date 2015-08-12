using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class BanUserFromSub : ICommandReturns<BanUserFromSub>, ICommandReturns<BanUserFromSubResponse>
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public Guid BannedBy { get; set; }

        public Guid? SubId { get; set; }

        public string SubName { get; set; }

        public DateTime DateBanned { get; set; }

        public DateTime? BannedUntil { get; set; }

        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }

    public class BanUserFromSubResponse
    {
        public string Error { get; set; }
    }
}
