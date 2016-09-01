using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    class _0009_UserAdminField : Migration
    {
        public _0009_UserAdminField() : base(MigrationType.Schema, 9)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE users ADD COLUMN is_admin boolean NOT NULL default false;");
            });
        }

        public override string GetDescription()
        {
            return "Add field to determine if a user is an admin.";
        }
    }
}
