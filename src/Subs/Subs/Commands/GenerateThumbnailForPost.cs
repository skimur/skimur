using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class GenerateThumbnailForPost : ICommand
    {
        public Guid PostId { get; set; }

        public bool Force { get; set; }
    }
}
