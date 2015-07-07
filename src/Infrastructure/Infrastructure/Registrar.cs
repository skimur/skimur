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
            container.RegisterSingle<IMapper, Mapper>();
            container.RegisterSingle<IConnectionStringProvider, ConnectionStringProvider>();
            container.RegisterSingle<IDbConnectionProvider, SqlConnectionProvider>();
            container.RegisterSingle<IEmailSender, EmailSender>();
        }

        public int Order { get { return 0; } }
    }
}
