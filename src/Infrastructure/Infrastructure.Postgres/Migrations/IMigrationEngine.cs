using Infrastructure.Data;

namespace Infrastructure.Postgres.Migrations
{
    public interface IMigrationEngine
    {
        bool Execute(IDbConnectionProvider conn, MigrationResources resources);
    }
}
