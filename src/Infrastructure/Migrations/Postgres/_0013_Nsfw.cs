﻿using System;
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
    class _0013_Nsfw : Migration
    {
        public _0013_Nsfw() : base(MigrationType.Schema, 13)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN nsfw boolean NOT NULL default false;");
                x.Execute("ALTER TABLE subs ADD COLUMN nsfw boolean NOT NULL default false;");
            });
        }

        public override string GetDescription()
        {
            return "Add a nsfw field to subs and posts.";
        }
    }
}
