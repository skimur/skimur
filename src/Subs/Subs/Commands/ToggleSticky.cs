using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;

namespace Subs.Commands
{
    public class ToggleSticky : ICommandReturns<ToggleStickyResponse>
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public bool Sticky { get; set; }
    }

    public class ToggleStickyResponse
    {
        public string Error { get; set; }
    }
}
