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
    public class _0007_CommentPostRepliesAndMentions : Migration
    {
        public _0007_CommentPostRepliesAndMentions() : base(MigrationType.Schema, 7)
        {

        }


        public override void Execute(IDbConnectionProvider conn)
        {
            conn.Perform(x =>
            {
                x.Execute("ALTER TABLE messages ADD COLUMN post uuid NULL;");
                x.Execute("ALTER TABLE messages ADD COLUMN comment uuid NULL;");
            });
        }

        public override string GetDescription()
        {
            return "Add a comment id and a post id to the messages table to be used for comment replies, post replies, and user mentions.";
        }
    }
}
