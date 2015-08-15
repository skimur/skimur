using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class UnBanUserFromSub : ICommandReturns<UnBanUserFromSubResponse>
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }
        
        public Guid? SubId { get; set; }

        public string SubName { get; set; }

        public Guid UnBannedBy { get; set; }
    }

    public class UnBanUserFromSubResponse
    {
        public string Error { get; set; }
    }
}
