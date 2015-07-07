using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Scheduling;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class EventBus : IEventBus
    {
        private readonly IBus _bus;

        public EventBus(IBus bus)
        {
            _bus = bus;
        }

        public void Publish<T>(T @event) where T : class, IEvent
        {
            _bus.Publish(@event);
        }

        public void Publish<T>(IEnumerable<T> events) where T : class, IEvent
        {
            foreach(var @event in events)
                _bus.Publish(@event);
        }
    }
}
