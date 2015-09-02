namespace Infrastructure.Postgres.Migrations
{
    public interface IMigrationResourceFinder
    {
        MigrationResources Find();
    }
}
