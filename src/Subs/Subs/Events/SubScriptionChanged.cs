using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Subs.Events
{
    public class SubScriptionChanged : IEvent
    {
        public bool Subcribed { get; set; }

        public bool Unsubscribed { get; set; }

        public string UserName { get; set; }

        public string SubName { get; set; }
    }
}
