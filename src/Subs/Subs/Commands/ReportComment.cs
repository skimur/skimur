using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class ReportComment : ICommand
    {
        public Guid ReportBy { get; set; }

        public Guid CommentId { get; set; }

        public string Reason { get; set; }
    }
}
