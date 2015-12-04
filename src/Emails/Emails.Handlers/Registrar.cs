using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emails.Commands;
using Emails.Handlers.Commands;
using Skimur;
using Skimur.Messaging.Handling;
using Microsoft.Extensions.DependencyInjection;

namespace Emails.Handlers
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICommandHandler<SendEmail>, EmailHandler>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
