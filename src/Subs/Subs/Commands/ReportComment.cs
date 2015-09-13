using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class ReportComment : ICommand
    {
        public Guid ReportBy { get; set; }

        public Guid CommentId { get; set; }

        public string Reason { get; set; }
    }
}
