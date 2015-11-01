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
    public class _0022_PostThumbnail : Migration
    {
        public _0022_PostThumbnail() : base(MigrationType.Schema, 22)
        {

        }

        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN thumb text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a column to store the thumbnail for a post.";
        }
    }
}
