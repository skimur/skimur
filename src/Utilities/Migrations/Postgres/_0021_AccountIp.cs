using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    class _0021_AccountIp : Migration
    {
        public _0021_AccountIp() : base(MigrationType.Schema, 21)
        {

        }
        
        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE users ADD COLUMN ip text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a column to store the IP of the registered user.";
        }
    }
}
