using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Commands
{
    public class EditSub : ICommandReturns<EditSubResponse>
    {
        public Guid EditedByUserId { get; set; }

        public string Name { get; set; }

        public string SidebarText { get; set; }

        public string Description { get; set; }
    }

    public class EditSubResponse
    {
        public string Error { get; set; }
    }
}
