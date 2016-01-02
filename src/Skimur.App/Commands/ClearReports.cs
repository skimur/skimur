using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class ClearReports : ICommand
    {
        public Guid UserId { get; set; }

        public Guid? CommentId { get; set; }

        public Guid? PostId { get; set; }
    }
}
