using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0012_PostDeleted : Migration
    {
        public _0012_PostDeleted() : base(MigrationType.Schema, 12)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN deleted boolean NOT NULL default false;");
            });
        }

        public override string GetDescription()
        {
            return "Add a deleted field to the posts.";
        }
    }
}
