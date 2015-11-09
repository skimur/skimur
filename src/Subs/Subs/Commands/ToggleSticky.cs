using System;
using Skimur.Messaging;

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
