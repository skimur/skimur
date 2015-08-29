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
            container.RegisterSingleton<IPathResolver, PathResolver>();
        }

        public int Order { get { return 0; } }
    }
}
