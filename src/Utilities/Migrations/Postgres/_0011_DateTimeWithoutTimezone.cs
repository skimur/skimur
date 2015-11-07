using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    class _0011_DateTimeWithoutTimezone : Migration
    {
        public _0011_DateTimeWithoutTimezone() : base(MigrationType.Schema, 11)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("DROP INDEX IF EXISTS comments_score_index;");
                x.Execute("DROP INDEX IF EXISTS comments_controversy_index;");
                x.Execute("DROP INDEX IF EXISTS posts_hot_index;");
                x.Execute("DROP INDEX IF EXISTS posts_score_index;");
                x.Execute("DROP INDEX IF EXISTS posts_controversy_index;");
                x.Execute("DROP INDEX IF EXISTS messages_datecreated_index;");

                x.Execute(@"
create or replace function hot(ups integer, downs integer, date timestamp without time zone) returns numeric as $$
    select round(cast(log(greatest(abs($1 - $2), 1)) * sign($1 - $2) + (date_part('epoch', $3) - 1134028003) / 45000.0 as numeric), 7)
$$ language sql immutable;");

                x.Execute("ALTER TABLE comments ALTER COLUMN date_created TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE comments ALTER COLUMN date_edited TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE posts ALTER COLUMN date_created TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE posts ALTER COLUMN last_edit_date TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE messages ALTER COLUMN date_created TYPE timestamp without time zone;");

                x.Execute("CREATE INDEX comments_score_index ON comments(hot(vote_up_count, vote_down_count, date_created), date_created);");
                x.Execute("CREATE INDEX comments_controversy_index ON comments(controversy(vote_up_count, vote_down_count), date_created);");
                x.Execute("CREATE INDEX posts_hot_index ON posts(hot(vote_up_count, vote_down_count, date_created), date_created);");
                x.Execute("CREATE INDEX posts_score_index ON posts(score(vote_up_count, vote_down_count), date_created);");
                x.Execute("CREATE INDEX posts_controversy_index ON posts(controversy(vote_up_count, vote_down_count), date_created);");
                x.Execute("CREATE INDEX messages_datecreated_index ON messages(date_created);");

                x.Execute("ALTER TABLE users ALTER COLUMN created_date TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE users ALTER COLUMN lockout_end_date TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE subs ALTER COLUMN created_date TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE moderators ALTER COLUMN added_on TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE votes ALTER COLUMN date_created TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE votes ALTER COLUMN date_casted TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE sub_user_bans ALTER COLUMN banned_until TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE sub_user_bans ALTER COLUMN date_banned TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE reported_comments ALTER COLUMN created_date TYPE timestamp without time zone;");
                x.Execute("ALTER TABLE reported_posts ALTER COLUMN created_date TYPE timestamp without time zone;");
            });
        }

        public override string GetDescription()
        {
            return "Change our date/time fields to be utc.";
        }
    }
}
