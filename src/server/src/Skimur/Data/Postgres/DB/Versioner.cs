using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Dapper;
using ServiceStack.Text;
using System.Threading.Tasks;

namespace Skimur.Data.Postgres.DB
{
    internal class Versioner : IVersioner
    {
        private readonly IDbConnectionProvider _conn;
        private readonly ILogger<Versioner> _logger;
        private bool _checkedTable;

        public Versioner(IDbConnectionProvider connectionProvider, ILogger<Versioner> logger)
        {
            _conn = connectionProvider;
            _logger = logger;
        }

        public async Task<int> CurrentVersion(MigrationType type)
        {
            await EnsureDatabaseCreated();

            var dbVersion = (await _conn.Perform(conn => conn.Query<DatabaseVersion>("select * from database_version")))
                .Where(x => x.Type == type)
                .OrderByDescending(x => x.Version)
                .Take(1)
                .FirstOrDefault();

            if (dbVersion == null)
            {
                _logger.LogInformation("No entries in database version table. Defaulting version value to 0.");
                return 0;
            }

            return dbVersion.Version;
        }

        public async Task<bool> SetVersion(Migration migration)
        {
            await EnsureDatabaseCreated();

            try
            {
                _logger.LogDebug("Updating database version to " + migration.Version);
                
                await _conn.Perform(conn => conn.Execute("insert into database_version(type, version, description, timestamp) values(@Type, @Version, @Description, @Timestamp)", new DatabaseVersion
                {
                    Type = migration.Type,
                    Version = migration.Version,
                    Description = migration.GetDescription(),
                    Timestamp = DateTime.Now.ToUnixTime()
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute insert of migration details into database version table", ex);
                return false;
            }

            return true;
        }

        private Task EnsureDatabaseCreated()
        {
            if (_checkedTable) Task.FromResult(0);

            _checkedTable = true;

            _logger.LogDebug("Create mapper and table instances");
            return _conn.Perform(conn =>
            {
                conn.Execute(
                    @"CREATE TABLE IF NOT EXISTS database_version
                    (
                        type text NOT NULL,
                        version integer NOT NULL,
                        timestamp bigint NOT NULL,
                        description text
                    )"
                );
            });
        }
    }
}
