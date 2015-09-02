using System;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Infrastructure.Logging;
using Skimur;

namespace Infrastructure.Cassandra.Migrations.DB
{
    internal class Versioner : IVersioner
    {
        private readonly IMapper _mapper;
        private readonly ILogger<Version> _logger;

        public Versioner(ISession session, ILogger<Version> logger)
        {
            _logger = logger;

            _logger.Debug("Define global mappings");
            MappingConfiguration.Global.Define<PocoMapper>();

            _logger.Debug("Create mapper and table instances");
            _mapper = new Mapper(session);
            var table = new Table<DatabaseVersion>(session);
            table.CreateIfNotExists();
        }

        public int CurrentVersion(MigrationType type)
        {
            var dbVersion = _mapper.FirstOrDefault<DatabaseVersion>("WHERE type = ?", (int)type);

            if (dbVersion == null)
            {
                _logger.Info("No entries in database version table. Defaulting version value to 0.");
                return 0;
            }

            return dbVersion.Version;
        }

        public bool SetVersion(Migration migration)
        {
            try
            {
                _logger.Debug("Updating database version to " + migration.Version);
                _mapper.Insert(new DatabaseVersion
                {
                    Type = migration.Type,
                    Version = migration.Version,
                    Description = migration.GetDescription(),
                    Timestamp = DateTime.Now.ToUnixTime()
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to execute insert of migration details into database version table", ex);
                return false;
            }

            return true;
        }
    }
}
