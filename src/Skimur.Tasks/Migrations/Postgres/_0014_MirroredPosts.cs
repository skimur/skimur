using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0014_MirroredPosts : Migration
    {
        public _0014_MirroredPosts() : base(MigrationType.Schema, 14)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN mirrored text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a field to indicate if a post is mirrored and where it is mirrored from.";
        }
    }
}
