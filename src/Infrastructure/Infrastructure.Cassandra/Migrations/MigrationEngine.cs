using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Infrastructure.Cassandra.Migrations.DB;
using Infrastructure.Logging;

namespace Infrastructure.Cassandra.Migrations
{
    /// <summary>
    /// Migration engine that executes data and schema migrations
    /// </summary>
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
