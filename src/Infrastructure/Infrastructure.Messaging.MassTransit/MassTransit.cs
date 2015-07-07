using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Magnum.Reflection;
using MassTransit;
using MassTransit.Saga;
using MassTransit.SubscriptionConfigurators;
using SimpleInjector;

namespace Infrastructure.Messaging.MassTransit
{
    public class MassTransit : IMassTransit
    {
        private readonly Container _container;
        private IServiceBus _serviceBus;

        public MassTransit(ICommandHandlerRegistry commandRegistry, IEventHandlerRegistry eventRegistry, Container container)
        {
            _container = container;
            _serviceBus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseRabbitMq();
                sbc.ReceiveFrom("rabbitmq://192.168.10.201/mybus");

                sbc.Subscribe(x =>
                {
                    eventRegistry.GetEvents((commandType) =>
                    {
                        RegisterHandler(x, commandType);
                    });
                });
            });
        }

        public void Publish<T>(T command) where T : class
        {
            _serviceBus.Publish(command);
        }

        public void Dispose()
        {
            _serviceBus.Dispose();
        }

        private void RegisterHandler(SubscriptionBusServiceConfigurator sub, Type type)
        {
            var methodInfo = typeof(MassTransit).GetMethod("Handler", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            var actionType = typeof(Action<>).MakeGenericType(type);
            var handler = Delegate.CreateDelegate(actionType, this, methodInfo);

            var busHandlerMethod = typeof(HandlerSubscriptionExtensions).GetMethods().First(x =>
            {
                if (x.Name == "Handler")
                {
                    // todo: ensure the right overloaded method
                    return true;
                }
                return false;
            }).MakeGenericMethod(type);
            busHandlerMethod.Invoke(null, new object[] { sub, handler });
        }

        private void Handler<T>(T message) where T : IEvent
        {
            _container.GetInstance<IEventHandler<T>>().Handle(message);
        }
    }
}
