using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace Infrastructure.Cassandra.Migrations
{
    public interface IMigrationEngine
    {
        bool Execute(ISession session, MigrationResources resources);
    }
}
