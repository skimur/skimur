using System;
using Skimur.Data;
using Skimur.Logging;
using Skimur.Postgres.Migrations.DB;

namespace Skimur.Postgres.Migrations
{
    public class MigrationEngine : IMigrationEngine
    {
        private readonly ILogger<MigrationEngine> _logger;

        public MigrationEngine(ILogger<MigrationEngine> logger)
        {
            _logger = logger;
        }

        public bool Execute(IDbConnectionProvider conn, MigrationResources resources)
        {
            _logger.Debug("Initialize database versioner");
            var versioner = new Versioner(conn, new Logger<Version>());

            _logger.Debug("Initialize executor");
            var executor = new Executor(conn, versioner, new Logger<Executor>());

            _logger.Debug("Execute migrations");
            return executor.Execute(resources);
        }
    }
}
