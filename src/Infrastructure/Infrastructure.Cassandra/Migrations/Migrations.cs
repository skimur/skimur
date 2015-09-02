using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using SimpleInjector;

namespace Infrastructure.Cassandra.Migrations
{
    public static class Migrations
    {
        public static void Run(Container container)
        {
            var migrations = container.GetInstance<IMigrationResourceFinder>().Find();
            container.GetInstance<IMigrationEngine>().Execute(container.GetInstance<ISession>(), migrations);
        }
    }
}
