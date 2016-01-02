using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
{
    public class _0010_PostContentFormatted : Migration
    {
        public _0010_PostContentFormatted() : base(MigrationType.Schema, 10)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN content_formatted text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a pre-renderer content field to the post table.";
        }
    }
}
