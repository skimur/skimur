using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace Infrastructure.Messaging.Handling
{
    public class EventDiscovery : IEventDiscovery
    {
        private readonly Container _container;

        public EventDiscovery(Container container)
        {
            _container = container;
        }

        public void Register(IEventRegistrar registrar)
        {
            var registrations = _container.GetCurrentRegistrations();

            foreach (var registration in registrations)
            {
                if (typeof (IEventHandler).IsAssignableFrom(registration.ServiceType))
                {
                    var genericArguements = registration.ServiceType.GenericTypeArguments.ToList();
                    if(genericArguements.Count == 0)
                        throw new Exception("The service " + registration.ServiceType + " must by of type IEventHandler<T>");
                    
                    RegisterEventHandler(genericArguements[0], registrar);
                }
            }
        }

        private void RegisterEventHandler(Type type, IEventRegistrar registrar)
        {
            var methodInfo = typeof(IEventRegistrar).GetMethod("RegisterEvent", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(type);
            methodInfo.Invoke(registrar, new object[0]);
        }
    }
}
