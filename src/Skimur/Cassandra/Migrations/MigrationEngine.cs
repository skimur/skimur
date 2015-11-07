using System;
using Cassandra;
using Skimur.Cassandra.Migrations.DB;
using Skimur.Logging;

namespace Skimur.Cassandra.Migrations
{
    public class MigrationEngine : IMigrationEngine
    {
        private readonly ILogger<MigrationEngine> _logger;

        public MigrationEngine(ILogger<MigrationEngine> logger)
        {
            _logger = logger;
        }

        public bool Execute(ISession session, MigrationResources resources)
        {
            _logger.Debug("Initialize database versioner");
            var versioner = new Versioner(session, new Logger<Version>());

            _logger.Debug("Initialize executor");
            var executor = new Executor(session, versioner, new Logger<Executor>());

            _logger.Debug("Execute migrations");
            return executor.Execute(resources);
        }
    }
}
