using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class GenerateThumbnailForPost : ICommand
    {
        public Guid PostId { get; set; }

        public bool Force { get; set; }
    }
}
