using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;

namespace Skimur.Messaging.RabbitMQ
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
            using (var client = (RabbitMqProducer) _server.CreateMessageQueueClient())
            {
                client.Publish(
                        QueueNames<T>.In,
                        new Message<T>(@event),
                        string.Concat(QueueNames.Exchange, ".", "events"));
            }
        }

        public void Publish<T>(IEnumerable<T> events) where T : class, IEvent
        {
            using (var client = (RabbitMqProducer)_server.CreateMessageQueueClient())
                foreach (var @event in events)
                {
                    client.Publish(
                        QueueNames<T>.In, 
                        new Message<T>(@event),
                        string.Concat(QueueNames.Exchange, ".", "events"));
                }
        }
    }
}
