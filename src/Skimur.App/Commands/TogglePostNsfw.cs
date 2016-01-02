using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class TogglePostNsfw : ICommand
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public bool IsNsfw { get; set; }
    }
}
