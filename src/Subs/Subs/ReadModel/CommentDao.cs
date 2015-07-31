using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Subs.Services;

namespace Subs.ReadModel
{
    public class CommentDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : CommentService, ICommentDao
    {
        private readonly ICommentTreeBuilder _commentTreeBuilder;

        public CommentDao(IDbConnectionProvider conn, ICommentTreeBuilder commentTreeBuilder)
            :base(conn)
        {
            _commentTreeBuilder = commentTreeBuilder;
        }

        public CommentTree GetCommentTree(string postSlug, CommentSortBy? sortBy = null)
        {
            return _commentTreeBuilder.GetCommentTree(postSlug, sortBy);
        }
    }
}
