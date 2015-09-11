using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class ConfigureReportIgnoring : ICommand
    {
        public Guid UserId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public bool IgnoreReports { get; set; }
    }
}
