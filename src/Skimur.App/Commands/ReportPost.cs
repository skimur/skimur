using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class ReportPost : ICommand
    {
        public Guid ReportBy { get; set; }

        public Guid PostId { get; set; }

        public string Reason { get; set; }
    }
}
