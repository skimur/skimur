using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Transports.RabbitMq;

namespace Infrastructure.Messaging.MassTransit
{
    public class EventBus : IEventBus
    {
        private readonly IMassTransit _massTransit;

        public EventBus(IMassTransit massTransit)
        {
            _massTransit = massTransit;
        }

        public void Publish(IEvent @event)
        {
            _massTransit.Publish(@event);
        }

        public void Publish(IEnumerable<IEvent> events)
        {

        }
    }
}
