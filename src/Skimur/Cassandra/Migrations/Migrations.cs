using Cassandra;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Cassandra.Migrations
{
    public static class Migrations
    {
        public static void Run(IServiceProvider serviceProvider)
        {
            var migrations = serviceProvider.GetService<IMigrationResourceFinder>().Find();
            serviceProvider.GetService<IMigrationEngine>().Execute(serviceProvider.GetService<ISession>(), migrations);
        }
    }
}
