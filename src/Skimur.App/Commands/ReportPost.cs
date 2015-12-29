using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class ReportPost : ICommand
    {
        public Guid ReportBy { get; set; }

        public Guid PostId { get; set; }

        public string Reason { get; set; }
    }
}
