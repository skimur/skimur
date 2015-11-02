using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class GenerateThumbnailForPost : ICommand
    {
        public Guid PostId { get; set; }

        public bool Force { get; set; }
    }
}
