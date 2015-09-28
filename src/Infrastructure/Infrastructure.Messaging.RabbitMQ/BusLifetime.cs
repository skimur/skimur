using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Infrastructure.Logging;
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
        private readonly ILogger<BusLifetime> _logger;
         
        public BusLifetime(Container container, 
            RabbitMqServer server, 
            ICommandDiscovery commandDiscovery, 
            IEventDiscovery eventDiscovery,
            ILogger<BusLifetime> logger)
        {
            _container = container;
            _server = server;
            _logger = logger;
            _registrar = new Registrar(_server, _container, _logger);

            commandDiscovery.Register(_registrar);
            eventDiscovery.Register(_registrar);

            _server.DisablePriorityQueues = true;
            _server.DisablePublishingResponses = true;
            _logger.Debug("Starting RabbitMQ server");
            _server.Start();
        }
        
        public void Dispose()
        {
            _logger.Debug("Stopping RabbitMQ server");
            _server.Stop();
            _server.Dispose();
        }

        class Registrar : ICommandRegistrar, IEventRegistrar
        {
            private readonly RabbitMqServer _server;
            private readonly Container _container;
            private readonly ILogger<BusLifetime> _logger;

            public Registrar(RabbitMqServer server, Container container, ILogger<BusLifetime> logger)
            {
                _server = server;
                _container = container;
                _logger = logger;
            }

            public void RegisterEvent<T>() where T : class, IEvent
            {
                _logger.Debug("Registering event handler " + typeof(T).Name);
                _server.RegisterHandler<T>(message =>
                {
                    try
                    {
                        _container.GetInstance<IEventHandler<T>>().Handle(message.GetBody());
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error processing command.", ex);
                        throw;
                    }
                });
            }

            public void RegisterCommand<T>() where T : class, ICommand
            {
                _logger.Debug("Registering command handler " + typeof(T).Name);
                _server.RegisterHandler<T>(message =>
                {
                    try
                    {
                        _container.GetInstance<ICommandHandler<T>>().Handle(message.GetBody());
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error processing command.", ex);
                        throw;
                    }
                });
            }

            public void RegisterCommandResponse<TRequest, TResponse>() where TRequest : class, ICommand, ICommandReturns<TResponse> where TResponse : class
            {
                _logger.Debug("Registering command handler " + typeof(TRequest).Name + " with response " + typeof(TResponse).Name);
                _server.RegisterHandler<TRequest>(message =>
                {
                    try
                    {
                        return  _container.GetInstance<ICommandHandlerResponse<TRequest, TResponse>>().Handle(message.GetBody());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error processing command.", ex);
                        throw;
                    }
                });
            }
        }
    }
}
