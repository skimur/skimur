using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class SubcribeToSub : ICommandReturns<SubcribeToSubResponse>
    {
        public string UserName { get; set; }

        public string SubName { get; set; }

        public Guid? SubId { get; set; }
    }

    public class SubcribeToSubResponse
    {
        public string Error { get; set; }

        public bool Success { get; set; }
    }
}
