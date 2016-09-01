namespace Skimur.Data.Postgres
{
    public interface IMigrationResourceFinder
    {
        MigrationResources Find();
    }
}
