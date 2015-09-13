using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Postgres.Migrations;
using ServiceStack.OrmLite.Dapper;

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
                x.Execute("ALTER TABLE moderators ADD COLUMN permissions integer DEFAULT " + (int)ModeratorPermissions.All + ";");
            });
        }

        public override string GetDescription()
        {
            return "Rename the 'sub_admins' table to 'moderators'. Also, add a permissions field to the table.";
        }

        [Flags]
        enum ModeratorPermissions
        {
            Access = 1,
            Config = 1 << 1,
            Flair = 1 << 2,
            Mail = 1 << 3,
            Posts = 1 << 4,
            All = Access | Config | Flair | Mail | Posts
        }
    }
}
