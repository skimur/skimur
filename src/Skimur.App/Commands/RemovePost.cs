using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class RemovePost : ICommandReturns<RemovePostResponse>
    {
        public Guid PostId { get; set; }

        public Guid RemovedBy { get; set; }
    }

    public class RemovePostResponse
    {
        public string Error { get; set; }
    }
}
