﻿using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
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
