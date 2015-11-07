using Skimur.Data;

namespace Skimur.Postgres.Migrations
{
    public interface IMigrationEngine
    {
        bool Execute(IDbConnectionProvider conn, MigrationResources resources);
    }
}
