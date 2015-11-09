using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class UnSubcribeToSub : ICommandReturns<UnSubcribeToSubResponse>
    {
        public Guid UserId { get; set; }

        public string SubName { get; set; }
    }

    public class UnSubcribeToSubResponse
    {
        public string Error { get; set; }

        public bool Success { get; set; }
    }
}
