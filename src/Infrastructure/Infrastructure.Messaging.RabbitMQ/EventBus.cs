using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.RabbitMq;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class EventBus : IEventBus
    {
        private readonly RabbitMqServer _server;

        public EventBus(RabbitMqServer server)
        {
            _server = server;
        }

        public void Publish<T>(T @event) where T : class, IEvent
        {
            using (var client = _server.CreateMessageQueueClient())
                client.Publish(@event);
        }

        public void Publish<T>(IEnumerable<T> events) where T : class, IEvent
        {
            using (var client = _server.CreateMessageQueueClient())
                foreach (var @event in events)
                    client.Publish(@event);
        }
    }
}
