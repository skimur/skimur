using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra.Mapping;

namespace Infrastructure.Cassandra.Migrations.DB
{
    internal class PocoMapper : Mappings
    {
        public PocoMapper()
        {
            For<DatabaseVersion>()
                .TableName("database_version")
                .PartitionKey("type")
                .ClusteringKey("version")
                .Column(c => c.Type, cm => cm.WithName("type").WithDbType(typeof(int)))
                .Column(c => c.Version, cm => cm.WithName("version"))
                .Column(c => c.Timestamp, cm => cm.WithName("timestamp"))
                .Column(c => c.Description, cm => cm.WithName("description"));
        }
    }
}
