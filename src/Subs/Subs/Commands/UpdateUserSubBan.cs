using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class UpdateUserSubBan : ICommandReturns<UpdateUserSubBanResponse>
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public Guid UpdatedBy { get; set; }

        public Guid? SubId { get; set; }

        public string SubName { get; set; }

        public string ReasonPrivate { get; set; }
    }

    public class UpdateUserSubBanResponse
    {
        public string Error { get; set; }

        public Guid UserId { get; set; }
    }
}
