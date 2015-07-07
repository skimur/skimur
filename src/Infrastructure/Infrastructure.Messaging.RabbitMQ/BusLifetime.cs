using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Infrastructure.Messaging.Handling;
using SimpleInjector;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class BusLifetime : IBusLifetime
    {
        private readonly Container _container;
        private readonly IBus _bus;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public BusLifetime(Container container, IBus bus, ICommandBus commandBus, IEventBus eventBus, ICommandHandlerRegistry commandHandlerRegistry, IEventHandlerRegistry eventHandlerRegistry)
        {
            _container = container;
            _bus = bus;
            _commandBus = commandBus;
            _eventBus = eventBus;

            commandHandlerRegistry.GetCommands(type =>
            {
                var genericArguements = type.GetGenericArguments();
                if (genericArguements.Count() == 1)
                {
                    RegisterCommandHandler(genericArguements[0]);
                }
                else
                {
                    RegisterCommandResponseHandler(genericArguements[0], genericArguements[1]);
                }
            });
            eventHandlerRegistry.GetEvents(RegisterEventHandler);
        }

        private void RegisterCommandHandler(Type type)
        {
            var methodInfo = typeof (BusLifetime).GetMethod("RegisterCommandHandlerGeneric", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            methodInfo.Invoke(this, new object[0]);
        }

        private void RegisterCommandHandlerGeneric<T>() where T : class, ICommand
        {
            _bus.Subscribe<T>(typeof (T).Name, command =>
            {
                _container.GetInstance<ICommandHandler<T>>().Handle(command);
            });
        }

        private void RegisterCommandResponseHandler(Type request, Type response)
        {
            var methodInfo = typeof(BusLifetime).GetMethod("RegisterCommandResponseHandlerGeneric", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(request, response);
            methodInfo.Invoke(this, new object[0]);
        }

        private void RegisterCommandResponseHandlerGeneric<TRequest, TResponse>() where TRequest : class, ICommandReturns<TResponse> where TResponse : class
        {
            _bus.Respond<TRequest, TResponse>(request => _container.GetInstance<ICommandHandlerResponse<TRequest, TResponse>>().Handle(request));
        }

        private void CommandHandler<T>(T message) where T : ICommand
        {
            _container.GetInstance<ICommandHandler<T>>().Handle(message);
        }

        private void CommandHandlerResponse<TRequest, TResponse>(TRequest message) where TRequest : ICommandReturns<TResponse>
        {
            _container.GetInstance<ICommandHandlerResponse<TRequest, TResponse>>().Handle(message);
        }

        private void RegisterEventHandler(Type type)
        {
            var methodInfo = typeof(BusLifetime).GetMethod("RegisterEventHandlergGeneric", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            methodInfo.Invoke(this, new object[0]);
        }

        private void RegisterEventHandlergGeneric<T>() where T : class, IEvent
        {
            _bus.Subscribe<T>(typeof (T).Name, @event =>
            {
                _container.GetInstance<IEventHandler<T>>().Handle(@event);
            });
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}
