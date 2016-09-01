using Microsoft.Extensions.Logging;
using Skimur.Data.Postgres.DB;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Data.Postgres
{
    internal class Executor
    {
        private readonly IDbConnectionProvider _conn;
        private readonly IVersioner _versioner;
        private readonly ILogger<Executor> _logger;

        public Executor(IDbConnectionProvider conn, IVersioner versioner, ILogger<Executor> logger)
        {
            _conn = conn;
            _versioner = versioner;
            _logger = logger;
        }

        public async Task<bool> Execute(MigrationResources resources)
        {
            _logger.LogDebug("Start executing migrations");

            foreach(var migration in resources.Migrations.OrderBy(x => x.Version))
            {
                var result = await Execute(migration);

                if (!result) return false;
            }

            return true;
        }

        private async Task<bool> Execute(Migration migration)
        {
            var type = migration.Type;
            var version = migration.Version;
            var dbVersion = await _versioner.CurrentVersion(type);

            _logger.LogInformation(string.Format("Database {0} version is : {1}", type, dbVersion));

            if (version <= dbVersion)
            {
                _logger.LogDebug(string.Format("{0} migration version {1} is less than database version", type, version));
                return true;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            _logger.LogInformation("Executing migration to version " + version);
            try
            {
                migration.Execute(_conn);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Failed to execute {0} migration to version {1}", type, version), ex);
                return false;
            }

            sw.Stop();
            _logger.LogInformation(string.Format("migration to version {0} took {1} seconds.", version, sw.Elapsed.TotalSeconds));

            if (!await _versioner.SetVersion(migration))
            {
                _logger.LogError("Failed to update database version. Leaving...");
                return false;
            }

            return true;
        }
    }
}
