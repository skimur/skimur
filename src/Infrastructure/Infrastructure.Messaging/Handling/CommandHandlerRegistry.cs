using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace Infrastructure.Messaging.Handling
{
    public class CommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly Container _container;

        public CommandHandlerRegistry(Container container)
        {
            _container = container;
        }

        public void GetCommands(Action<Type> command)
        {
            var registrations = _container.GetCurrentRegistrations();

            foreach (var registration in registrations)
            {
                if (typeof (ICommandHandler).IsAssignableFrom(registration.ServiceType))
                {
                    command(registration.ServiceType);
                }
            }
        }
    }
}
