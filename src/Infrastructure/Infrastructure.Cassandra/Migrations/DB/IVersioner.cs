using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations.DB
{
    internal interface IVersioner
    {
        int CurrentVersion(MigrationType type);

        bool SetVersion(Migration migration);
    }
}
