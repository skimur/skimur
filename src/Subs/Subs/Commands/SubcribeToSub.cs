using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class SubcribeToSub : ICommandReturns<SubcribeToSubResponse>
    {
        public string UserName { get; set; }

        public string SubName { get; set; }
    }

    public class SubcribeToSubResponse
    {
        public string Error { get; set; }

        public bool Success { get; set; }
    }
}
