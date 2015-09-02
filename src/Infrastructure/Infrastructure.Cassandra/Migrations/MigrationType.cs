using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations
{
    public enum MigrationType : short
    {
        Data = 0,
        Schema = 1
    }
}
