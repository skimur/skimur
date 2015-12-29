using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class GenerateEmbeddedMediaObject : ICommand
    {
        public Guid PostId { get; set; }

        public bool Force { get; set; }
    }
}
