using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations
{
    public interface IMigrationResourceFinder
    {
        MigrationResources Find();
    }
}
