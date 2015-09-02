using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace Infrastructure.Cassandra.Migrations
{
    /// <summary>
    /// Schema migration abstraction that should be implemented for each database migration
    /// </summary>
    public abstract class Migration
    {
        protected Migration(MigrationType type, int version)
        {
            Type = type;
            Version = version;
        }

        /// <summary>
        /// Migration type
        /// </summary>
        public MigrationType Type { get; private set; }

        /// <summary>
        /// Migration version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Executes migration
        /// </summary>
        /// <param name="session">Datastax driver session instance</param>
        public abstract void Execute(ISession session);

        /// <summary>
        /// Gets migration description
        /// </summary>
        /// <returns>description text</returns>
        public abstract string GetDescription();
    }
}
