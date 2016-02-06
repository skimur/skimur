using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Postgres.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Tasks.Migrations.Postgres
{
    public class _0024_ScreenIps : Migration
    {
        public _0024_ScreenIps() : base(MigrationType.Schema, 24)
        {

        }

        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.ExecuteSql(@"
CREATE TABLE screened_ip_addresses
(
    id uuid NOT NULL,
    ip inet NOT NULL,
    action integer NOT NULL,
    number_of_matches integer DEFAULT 0 NOT NULL,
    last_match_at timestamp without time zone,
    created_on timestamp without time zone NOT NULL,
    updated_on timestamp without time zone NOT NULL
);
                    ");
            });
        }

        public override string GetDescription()
        {
            return "Add fields to the subs table for submission text and sidebar text.";
        }


    }
}
