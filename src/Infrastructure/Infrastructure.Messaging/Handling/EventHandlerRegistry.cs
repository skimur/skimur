using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace Infrastructure.Messaging.Handling
{
    public class EventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Container _container;

        public EventHandlerRegistry(Container container)
        {
            _container = container;
        }

        public void GetEvents(Action<Type> command)
        {
            var registrations = _container.GetCurrentRegistrations();

            foreach (var registration in registrations)
            {
                if (typeof (IEventHandler).IsAssignableFrom(registration.ServiceType))
                {
                    var genericArguements = registration.ServiceType.GenericTypeArguments.ToList();
                    if(genericArguements.Count == 0)
                        throw new Exception("The service " + registration.ServiceType + " must by of type IEventHandler<T>");

                    command(genericArguements[0]);
                }
            }
        }
    }
}
