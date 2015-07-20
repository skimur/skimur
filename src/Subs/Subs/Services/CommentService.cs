using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ServiceStack.OrmLite;

namespace Subs.Services
{
    public class CommentService : ICommentService
    {
        private readonly IDbConnectionProvider _conn;

        public CommentService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public Comment GetCommentById(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return _conn.Perform(conn => conn.SingleById<Comment>(id));
        }

        public void InsertComment(Comment comment)
        {
            _conn.Perform(conn =>
            {
                conn.Insert(comment);
            });
        }

        public List<Comment> GetAllCommentsForPost(string postSlug)
        {
            if(string.IsNullOrEmpty(postSlug))
                return new List<Comment>();

            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<Comment>().Where(x => x.PostSlug == postSlug));
            });
        }
    }
}
