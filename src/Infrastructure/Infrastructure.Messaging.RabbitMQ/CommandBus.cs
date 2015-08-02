using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class CommandBus : ICommandBus
    {
        private readonly RabbitMqServer _server;

        public CommandBus(RabbitMqServer server)
        {
            _server = server;
        }

        public void Send<T>(T command) where T : class, ICommand
        {
            using (var client = _server.CreateMessageQueueClient())
                client.Publish(command);
        }

        public void Send<T>(IEnumerable<T> commands) where T : class, ICommand
        {
            using (var client = _server.CreateMessageQueueClient())
                foreach(var command in commands)
                    client.Publish(command);
        }

        public TResponse Send<TRequest, TResponse>(TRequest command)
            where TRequest : class, ICommandReturns<TResponse>
            where TResponse : class
        {
            using (var client = _server.CreateMessageQueueClient())
            {
                string replyToMq = client.GetTempQueueName();
                client.Publish(new Message<TRequest>(command)
                {
                    ReplyTo = replyToMq
                });

                IMessage<TResponse> responseMsg = client.Get<TResponse>(replyToMq);
                client.Ack(responseMsg);
                return responseMsg.GetBody();
            }
        }
    }
}
