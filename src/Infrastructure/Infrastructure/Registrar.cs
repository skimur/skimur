using System;
using Infrastructure.Data;
using Infrastructure.Email;
using SimpleInjector;
using Skimur;

namespace Infrastructure
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<IMapper, Mapper>();
            container.RegisterSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            container.RegisterSingleton<IDbConnectionProvider, SqlConnectionProvider>();
            container.RegisterSingleton<IEmailSender, EmailSender>();
        }

        public int Order { get { return 0; } }
    }
}
