using SimpleInjector;
using Skimur.Data;

namespace Skimur.Postgres.Migrations
{
    public static class Migrations
    {
        public static void Run(Container container)
        {
            var migrations = container.GetInstance<IMigrationResourceFinder>().Find();
            container.GetInstance<IMigrationEngine>().Execute(container.GetInstance<IDbConnectionProvider>(), migrations);
        }
    }
}
