using Cassandra;

namespace Skimur.Cassandra.Migrations
{
    public interface IMigrationEngine
    {
        bool Execute(ISession session, MigrationResources resources);
    }
}
