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
    class _0008_SubCreatedBy : Migration
    {
        public _0008_SubCreatedBy() : base(MigrationType.Schema, 8)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE subs ADD COLUMN created_by uuid NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a field to keep track of who created what sub.";
        }
    }
}
