using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class ReportComment : ICommand
    {
        public Guid ReportBy { get; set; }

        public Guid CommentId { get; set; }

        public string Reason { get; set; }
    }
}
