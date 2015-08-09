using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class SubEventHandler : IEventHandler<SubScriptionChanged>
    {
        private readonly ISubService _subService;

        public SubEventHandler(ISubService subService)
        {
            _subService = subService;
        }

        public void Handle(SubScriptionChanged @event)
        {
            ulong numberOfSubscribers;
            _subService.UpdateNumberOfSubscribers(@event.SubId, out numberOfSubscribers);
        }
    }
}
