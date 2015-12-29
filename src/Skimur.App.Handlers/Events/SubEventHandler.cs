using Skimur.Messaging.Handling;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker.Events
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
