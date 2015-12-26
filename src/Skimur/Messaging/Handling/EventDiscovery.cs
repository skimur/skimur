using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Messaging.Handling
{
    public class EventDiscovery : IEventDiscovery
    {
        private readonly IServiceCollection _services;

        public EventDiscovery(IServiceCollection services)
        {
            _services = services;
        }

        public void Register(IEventRegistrar registrar)
        {
            foreach(var registration in _services.Select(x => x.ImplementationType))
            {
                if(typeof(IEventHandler).IsAssignableFrom(registration))
                {
                    foreach(var intface in registration.GetInterfaces().Where(x => typeof(IEventHandler).IsAssignableFrom(x)))
                    {
                        var genericArguements = intface.GenericTypeArguments.ToList();
                        if (genericArguements.Count == 0)
                            continue;

                        RegisterEventHandler(genericArguements[0], registration, registrar);
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
