using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0023_PostMediaExpando : Migration
    {
        public _0023_PostMediaExpando() : base(MigrationType.Schema, 23)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE posts ADD COLUMN media json NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a field to store the media object used for previewing media items.";
        }
    }
}
