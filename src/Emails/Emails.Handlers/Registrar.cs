using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emails.Commands;
using Emails.Handlers.Commands;
using Infrastructure.Messaging.Handling;
using SimpleInjector;
using Skimur;

namespace Emails.Handlers
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
           container.Register<ICommandHandler<SendEmail>, EmailHandler>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
