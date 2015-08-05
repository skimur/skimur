using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ServiceStack.OrmLite;
using Subs.Services;

namespace Subs.ReadModel
{
    public class CommentDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : CommentService, ICommentDao
    {
        private readonly IDbConnectionProvider _conn;
        private readonly ICommentTreeBuilder _commentTreeBuilder;
        private readonly ICommentTreeContextBuilder _commentTreeContextBuilder;

        public CommentDao(IDbConnectionProvider conn,
            ICommentTreeBuilder commentTreeBuilder,
            ICommentTreeContextBuilder commentTreeContextBuilder)
            : base(conn)
        {
            _conn = conn;
            _commentTreeBuilder = commentTreeBuilder;
            _commentTreeContextBuilder = commentTreeContextBuilder;
        }

        public CommentTree GetCommentTree(string postSlug)
        {
            return _commentTreeBuilder.GetCommentTree(postSlug);
        }

        public Dictionary<Guid, double> GetCommentTreeSorter(string postSlug, CommentSortBy sortBy)
        {
            // TODO: This should be cached and updated periodically

            if (string.IsNullOrEmpty(postSlug))
                return new Dictionary<Guid, double>();

            return _conn.Perform(conn =>
            {
                var query = conn.From<Comment>().Where(x => x.PostSlug == postSlug);

                switch (sortBy)
                {
                    case CommentSortBy.Best:
                        query.OrderBy(x => x.SortConfidence);
                        break;
                    case CommentSortBy.Top:
                        query.OrderByExpression = "ORDER BY (score(vote_up_count, vote_down_count), date_created)";
                        break;
                    case CommentSortBy.New:
                        query.OrderBy(x => x.DateCreated);
                        break; 
                    case CommentSortBy.Controversial:
                        query.OrderByExpression = "ORDER BY (controversy(vote_up_count, vote_down_count), date_created)";
                        break;
                    case CommentSortBy.Old:
                        query.OrderByDescending(x => x.DateCreated);
                        break;
                    case CommentSortBy.Qa:
                        query.OrderBy(x => x.SortQa);
                        break;
                    default:
                        throw new Exception("unknown sort");
                }

                query.SelectExpression = "SELECT \"id\"";
                
                var commentsSorted = conn.Select(query).Select(x => x.Id).ToList();

                return commentsSorted.ToDictionary(x => x, x => (double)commentsSorted.IndexOf(x));
            });
        }
    }
}
