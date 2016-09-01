using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0002_PostModeration : Migration
    {
        public _0002_PostModeration() : base(MigrationType.Schema, 2)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN verdict integer NOT NULL DEFAULT 0;");
                x.Execute("ALTER TABLE posts ADD COLUMN approved_by uuid NULL;");
                x.Execute("ALTER TABLE posts ADD COLUMN removed_by uuid NULL;");
                x.Execute("ALTER TABLE posts ADD COLUMN verdict_message text NULL;");
            });
        }
        
        public override string GetDescription()
        {
            return "Create the verdict/verdictmessage/approvedby/removedby properties for a post.";
        }
    }
}
