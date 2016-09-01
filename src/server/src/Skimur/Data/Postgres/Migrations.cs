using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Skimur.Data.Postgres
{
    public static class Migrations
    {
        public static Task<bool> Run(IServiceProvider serviceProvider)
        {
            var migrations = serviceProvider.GetService<IMigrationResourceFinder>().Find();
            return serviceProvider.GetService<IMigrationEngine>().Execute(serviceProvider.GetService<IDbConnectionProvider>(), migrations);
        }
    }
}
