using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Postgres.Migrations;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Postgres
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<IMigrationEngine, MigrationEngine>();
            container.RegisterSingleton<IMigrationResourceFinder, MigrationResourceFinder>();
        }

        public int Order { get { return 0; } }
    }
}
