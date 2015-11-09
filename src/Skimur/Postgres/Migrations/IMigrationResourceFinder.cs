namespace Skimur.Postgres.Migrations
{
    public interface IMigrationResourceFinder
    {
        MigrationResources Find();
    }
}
