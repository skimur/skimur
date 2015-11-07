using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class TogglePostNsfw : ICommand
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public bool IsNsfw { get; set; }
    }
}
