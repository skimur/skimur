using System;
using RabbitMQ.Client;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using SimpleInjector;
using Skimur.Logging;
using Skimur.Messaging.Handling;

namespace Skimur.Messaging.RabbitMQ
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

            public void RegisterEvent<T, TEventHandler>()
                where T : class, IEvent
                where TEventHandler : class, IEventHandler<T>
            {
                _logger.Debug("Registering event handler " + typeof(T).Name);

                var queueName = "mq:" + typeof(TEventHandler).Name + ":" + typeof(T).Name + ".inq";
                
                using (var messageProducer = (RabbitMqProducer) _server.CreateMessageProducer())
                {
                    using (var channel = messageProducer.Channel)
                    {
                        channel.ExchangeDeclare(string.Concat(QueueNames.Exchange, ".", "events"),
                                        ExchangeType.Direct,
                                        durable: true,
                                        autoDelete: false,
                                        arguments: null);
                        
                        channel.QueueDeclare(queueName, 
                            durable: true, 
                            exclusive: false, 
                            autoDelete: false, 
                            arguments: null);
                        
                        channel.QueueBind(queueName,
                                            string.Concat(QueueNames.Exchange, ".", "events"),
                                            QueueNames<T>.In);
                    }
                }

                new RabbitMqWorker((RabbitMqMessageFactory)_server.MessageFactory, 
                    new MessageHandler<T>(_server, message =>
                    {
                        _container.GetInstance<TEventHandler>().Handle(message.GetBody());
                        return null;
                    })
                    {
                        ProcessQueueNames = new[] { queueName }
                    },
                    queueName,
                    (worker, exception) =>
                    {
                        _logger.Error("Error from processing queue " + queueName, exception);
                    }).Start();
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
