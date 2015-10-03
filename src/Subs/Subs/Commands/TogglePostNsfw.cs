using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class TogglePostNsfw : ICommand
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public bool IsNsfw { get; set; }
    }
}
