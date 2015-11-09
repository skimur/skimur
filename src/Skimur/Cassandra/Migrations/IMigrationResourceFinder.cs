namespace Skimur.Cassandra.Migrations
{
    public interface IMigrationResourceFinder
    {
        MigrationResources Find();
    }
}
