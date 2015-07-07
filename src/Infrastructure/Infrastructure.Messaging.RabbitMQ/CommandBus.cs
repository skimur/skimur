using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.FluentConfiguration;
using Infrastructure.Messaging.Handling;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class CommandBus : ICommandBus
    {
        private readonly IBus _bus;

        public CommandBus(IBus bus)
        {
            _bus = bus;
        }

        public void Send<T>(T command) where T : class, ICommand
        {
            _bus.Publish(command);
        }

        public void Send<T>(IEnumerable<T> commands) where T : class, ICommand
        {
            foreach (var command in commands)
                _bus.Publish(command);
        }

        public TResponse Send<TRequest, TResponse>(TRequest command)
            where TRequest : class, ICommandReturns<TResponse>
            where TResponse : class
        {
            return _bus.Request<TRequest, TResponse>(command);
        }
    }
}
