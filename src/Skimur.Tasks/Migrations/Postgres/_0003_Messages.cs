using ServiceStack.OrmLite.Dapper;
using Skimur.Data;
using Skimur.Postgres.Migrations;

namespace Skimur.Tasks.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0003_Messages : Migration
    {
        public _0003_Messages() : base(MigrationType.Schema, 3)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute(@"
CREATE TABLE messages
(
  id uuid NOT NULL,
  date_created timestamp with time zone NOT NULL,
  type integer,
  parent_id uuid,
  first_message uuid,
  deleted boolean,
  author_id uuid NOT NULL,
  author_ip text,
  is_new boolean,
  to_user uuid,
  to_sub uuid,
  from_sub uuid,
  subject text,
  body text,
  body_formatted text,
  CONSTRAINT messages_pkey PRIMARY KEY (id)
);");

                x.Execute(@"
CREATE INDEX messages_datecreated_index ON messages(date_created);
");

                
            });
        }

        public override string GetDescription()
        {
            return "Create the messages table.";
        }
    }
}
