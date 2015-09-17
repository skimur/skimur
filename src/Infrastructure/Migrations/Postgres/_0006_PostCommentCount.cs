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
    public class _0006_PostCommentCount : Migration
    {
        public _0006_PostCommentCount() : base(MigrationType.Schema, 6)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN number_of_comments integer NOT NULL DEFAULT 0;");
            });
        }

        public override string GetDescription()
        {
            return "Create the number_of_comments column for a post.";
        }
    }
}
