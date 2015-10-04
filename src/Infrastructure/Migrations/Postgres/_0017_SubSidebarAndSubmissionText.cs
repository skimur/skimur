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
    public class _0017_SubSidebarAndSubmissionText : Migration
    {
        public _0017_SubSidebarAndSubmissionText() : base(MigrationType.Schema, 17)
        {

        }
        
        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE subs ADD COLUMN sidebar_text_formatted text NULL;");
                x.Execute("ALTER TABLE subs ADD COLUMN submission_text text NULL;");
                x.Execute("ALTER TABLE subs ADD COLUMN submission_text_formatted text NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add fields to the subs table for submission text and sidebar text.";
        }
    }
}
