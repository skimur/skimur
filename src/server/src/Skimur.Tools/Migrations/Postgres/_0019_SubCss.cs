using Dapper;
using Skimur.Data;
using Skimur.Data.Postgres;

namespace Skimur.Tools.Migrations.Postgres
{
    // ReSharper disable once InconsistentNaming
    public class _0019_SubCss : Migration
    {
        public _0019_SubCss() : base(MigrationType.Schema, 19)
        {

        }

        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute(@"
CREATE TABLE sub_css
(
  id uuid NOT NULL,
  sub_id uuid NOT NULL,
  type int NOT NULL,
  embedded text NULL,
  external_css text NULL,
  github_css_project_name text NULL,
  github_css_project_tag text NULL,
  github_less_project_name text NULL,
  github_less_project_tag text NULL,
  CONSTRAINT sub_css_pkey PRIMARY KEY (id)
);");
                x.Execute("ALTER TABLE users ADD COLUMN styles boolean NOT NULL default TRUE;");
            });
        }

        public override string GetDescription()
        {
            return "Add table to css settings for subs. Add a setting to the users table to indicate if the user wants to be presented with custom CSS.";
        }
    }
}
