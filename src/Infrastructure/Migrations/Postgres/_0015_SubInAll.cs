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
    public class _0015_SubInAll : Migration
    {
        public _0015_SubInAll() : base(MigrationType.Schema, 15)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE subs ADD COLUMN in_all boolean NOT NULL default TRUE;");
                x.Execute("ALTER TABLE posts ADD COLUMN in_all boolean NOT NULL default TRUE;");
            });
        }

        public override string GetDescription()
        {
            return "Add a field to indicate if this sub show show in /s/all.";
        }
    }
}
