using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations.DB
{
    internal class DatabaseVersion
    {
        public MigrationType Type { get; set; }
        
        public int Version { get; set; }

        public long Timestamp { get; set; }

        public string Description { get; set; }
    }
}
