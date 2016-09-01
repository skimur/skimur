using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    class _0021_AccountIp : Migration
    {
        public _0021_AccountIp() : base(MigrationType.Schema, 21)
        {

        }
        
        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE users ADD COLUMN ip text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a column to store the IP of the registered user.";
        }
    }
}
