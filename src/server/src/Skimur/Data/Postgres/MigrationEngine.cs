using Microsoft.Extensions.Logging;
using Skimur.Data.Postgres.DB;
using System;
using System.Threading.Tasks;

namespace Skimur.Data.Postgres
{
    public class MigrationEngine : IMigrationEngine
    {
        private readonly ILogger<MigrationEngine> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public MigrationEngine(ILogger<MigrationEngine> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public Task<bool> Execute(IDbConnectionProvider conn, MigrationResources resources)
        {
            _logger.LogDebug("Initialize database versioner");
            var versioner = new Versioner(conn, _loggerFactory.CreateLogger<Versioner>());

            _logger.LogDebug("Initialize executor");
            var executor = new Executor(conn, versioner, _loggerFactory.CreateLogger<Executor>());

            _logger.LogDebug("Execute migrations");
            return executor.Execute(resources);
        }
    }
}
