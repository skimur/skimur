using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    class _0008_SubCreatedBy : Migration
    {
        public _0008_SubCreatedBy() : base(MigrationType.Schema, 8)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE subs ADD COLUMN created_by uuid NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a field to keep track of who created what sub.";
        }
    }
}
