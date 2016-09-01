using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0020_StickyPosts : Migration
    {
        public _0020_StickyPosts() : base(MigrationType.Schema, 20)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN sticky boolean NOT NULL DEFAULT false;");
            });
        }

        public override string GetDescription()
        {
            return "Add a column to a post to indicate that it is stickied.";
        }
    }
}
