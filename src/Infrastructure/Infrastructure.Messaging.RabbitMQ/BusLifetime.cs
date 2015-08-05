using System;
using System.Linq;
using System.Reflection;
using Infrastructure.Messaging.Handling;
using ServiceStack.RabbitMq;
using SimpleInjector;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class BusLifetime : IBusLifetime
    {
        private readonly Container _container;
        private readonly RabbitMqServer _server;
        private readonly Registrar _registrar;

        public BusLifetime(Container container, RabbitMqServer server, ICommandDiscovery commandDiscovery, IEventDiscovery eventDiscovery)
        {
            _container = container;
            _server = server;
            _registrar = new Registrar(_server, _container);

            commandDiscovery.Register(_registrar);
            eventDiscovery.Register(_registrar);

            _server.DisablePriorityQueues = true;
            _server.Start();
        }
        
        public void Dispose()
        {
            _server.Stop();
            _server.Dispose();
        }

        class Registrar : ICommandRegistrar, IEventRegistrar
        {
            private readonly RabbitMqServer _server;
            private readonly Container _container;

            public Registrar(RabbitMqServer server, Container container)
            {
                _server = server;
                _container = container;
            }

            public void RegisterEvent<T>() where T : class, IEvent
            {
                _server.RegisterHandler<T>(message =>
                {
                    _container.GetInstance<IEventHandler<T>>().Handle(message.GetBody());
                    return null;
                });
            }

            public void RegisterCommand<T>() where T : class, ICommand
            {
                _server.RegisterHandler<T>(message =>
                {
                    _container.GetInstance<ICommandHandler<T>>().Handle(message.GetBody());
                    return null;
                });
            }

            public void RegisterCommandResponse<TRequest, TResponse>() where TRequest : class, ICommand, ICommandReturns<TResponse> where TResponse : class
            {
                _server.RegisterHandler<TRequest>(message => _container.GetInstance<ICommandHandlerResponse<TRequest, TResponse>>().Handle(message.GetBody()));
            }
        }
    }
}
