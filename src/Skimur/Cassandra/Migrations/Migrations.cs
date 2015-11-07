using Cassandra;
using SimpleInjector;

namespace Skimur.Cassandra.Migrations
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
