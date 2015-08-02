using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace Infrastructure.Messaging.Handling
{
    public class CommandDiscovery : ICommandDiscovery
    {
        private readonly Container _container;

        public CommandDiscovery(Container container)
        {
            _container = container;
        }

        public void Register(ICommandRegistrar registrar)
        {
            var registrations = _container.GetCurrentRegistrations();

            foreach (var registration in registrations)
            {
                if (typeof (ICommandHandler).IsAssignableFrom(registration.ServiceType))
                {
                    var genericArguements = registration.ServiceType.GetGenericArguments();
                    if (genericArguements.Count() == 1)
                        RegisterCommandHandler(genericArguements[0], registrar);
                    else
                        RegisterCommandResponseHandler(genericArguements[0], genericArguements[1], registrar);
                }
            }
        }

        private void RegisterCommandHandler(Type type, ICommandRegistrar registrar)
        {
            var methodInfo = typeof(ICommandRegistrar).GetMethod("RegisterCommand", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(type);
            methodInfo.Invoke(registrar, new object[0]);
        }

        private void RegisterCommandResponseHandler(Type request, Type response, ICommandRegistrar registrar)
        {
            var methodInfo = typeof(ICommandRegistrar).GetMethod("RegisterCommandResponse", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(request, response);
            methodInfo.Invoke(registrar, new object[0]);
        }
    }
}
