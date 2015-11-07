using System;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace Skimur.Messaging.Handling
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
                    // get all the implementations on this type
                    foreach(var intface in registration.ServiceType.GetInterfaces().Where(x => typeof(IEventHandler).IsAssignableFrom(x)))
                    {
                        var genericArguements = intface.GenericTypeArguments.ToList();
                        if (genericArguements.Count == 0)
                            continue;
                        
                        RegisterEventHandler(genericArguements[0], registration.Registration.ImplementationType, registrar);
                    }
                }
            }
        }

        private void RegisterEventHandler(Type type, Type implementation, IEventRegistrar registrar)
        {
            var methodInfo = typeof(IEventRegistrar).GetMethod("RegisterEvent", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(type, implementation);
            methodInfo.Invoke(registrar, new object[0]);
        }
    }
}
