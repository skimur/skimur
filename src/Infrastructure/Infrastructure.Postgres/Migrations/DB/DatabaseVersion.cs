using ServiceStack.DataAnnotations;

namespace Infrastructure.Postgres.Migrations.DB
{
    internal class DatabaseVersion
    {
        public MigrationType Type { get; set; }

        public int Version { get; set; }

        public long Timestamp { get; set; }

        public string Description { get; set; }
    }
}
