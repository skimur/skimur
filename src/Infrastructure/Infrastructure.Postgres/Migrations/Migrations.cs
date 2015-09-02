using Infrastructure.Data;
using SimpleInjector;

namespace Infrastructure.Postgres.Migrations
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
