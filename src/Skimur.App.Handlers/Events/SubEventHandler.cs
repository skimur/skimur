using Skimur.App.Events;
using Skimur.App.Services;
using Skimur.Messaging.Handling;

namespace Skimur.App.Handlers.Events
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
