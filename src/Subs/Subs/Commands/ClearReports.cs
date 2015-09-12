using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;

namespace Subs.Commands
{
    public class ClearReports : ICommand
    {
        public Guid UserId { get; set; }

        public Guid? CommentId { get; set; }

        public Guid? PostId { get; set; }
    }
}
