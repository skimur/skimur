using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations
{
    /// <summary>
    /// Defines type of database migration
    /// </summary>
    public enum MigrationType : short
    {
        /// <summary>
        /// Schema migration
        /// </summary>
        Schema = 1,

        /// <summary>
        /// Data migration
        /// </summary>
        Data
    }
}
