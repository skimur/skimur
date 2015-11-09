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
    public class _0005_ModeratorPermissions : Migration
    {
        public _0005_ModeratorPermissions() : base(MigrationType.Schema, 5)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE sub_admins RENAME TO moderators");
                // "1" is "All" permission
                x.Execute("ALTER TABLE moderators ADD COLUMN permissions integer DEFAULT 1");
            });
        }

        public override string GetDescription()
        {
            return "Rename the 'sub_admins' table to 'moderators'. Also, add a permissions field to the table.";
        }
    }
}
