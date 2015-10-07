using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Data;
using ServiceStack.OrmLite;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services.Impl
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

        public List<Comment> GetCommentsByIds(List<Guid> ids)
        {
            if(ids == null || ids.Count == 0)
                return new List<Comment>();

            return _conn.Perform(conn => conn.SelectByIds<Comment>(ids));
        }

        public void InsertComment(Comment comment)
        {
            _conn.Perform(conn =>
            {
                conn.Insert(comment);
            });
        }

        public void UpdateCommentBody(Guid commentId, string body, string bodyFormatted, DateTime dateEdited)
        {
            _conn.Perform(conn =>
            {
                conn.Update<Comment>(new
                {
                    Body = body,
                    BodyFormatted = bodyFormatted,
                    DateEdited = dateEdited
                }, x => x.Id == commentId);
            });
        }

        public List<Comment> GetAllCommentsForPost(Guid postId, CommentSortBy? sortBy = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Comment>().Where(x => x.PostId == postId);

                if (sortBy.HasValue)
                {
                    switch (sortBy)
                    {
                        case CommentSortBy.Best:
                            query.OrderByDescending(x => x.SortConfidence);
                            break;
                        case CommentSortBy.Top:
                            query.OrderByExpression = "ORDER BY (score(vote_up_count, vote_down_count), date_created) DESC";
                            break;
                        case CommentSortBy.New:
                            query.OrderByDescending(x => x.DateCreated);
                            break;
                        case CommentSortBy.Controversial:
                            query.OrderByExpression = "ORDER BY (controversy(vote_up_count, vote_down_count), date_created) DESC";
                            break;
                        case CommentSortBy.Old:
                            query.OrderBy(x => x.DateCreated);
                            break;
                        case CommentSortBy.Qa:
                            query.OrderByDescending(x => x.SortQa);
                            break;
                        default:
                            throw new Exception("unknown sort");
                    }
                }

                return conn.Select(query);
            });
        }

        public List<Comment> GetChildrenForComment(Guid commentId, Guid? authorId = null)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(authorId.HasValue ? conn.From<Comment>().Where(x => x.ParentId == commentId && x.AuthorUserId == authorId) : conn.From<Comment>().Where(x => x.ParentId == commentId));
            });
        }
        
        public void UpdateCommentVotes(Guid commentId, int? upVotes, int? downVotes)
        {
            if (downVotes.HasValue || upVotes.HasValue)
            {
                _conn.Perform(conn =>
                {
                    if (upVotes.HasValue && downVotes.HasValue)
                    {
                        conn.Update<Comment>(new { VoteUpCount = upVotes.Value, VoteDownCount = downVotes.Value }, x => x.Id == commentId);
                    }
                    else if (upVotes.HasValue)
                    {
                        conn.Update<Comment>(new { VoteUpCount = upVotes.Value }, x => x.Id == commentId);
                    }
                    else if (downVotes.HasValue)
                    {
                        conn.Update<Comment>(new { VoteDownCount = downVotes.Value }, x => x.Id == commentId);
                    }
                });
            }
        }

        public void UpdateCommentSorting(Guid commentId, double? confidence, double? qa)
        {
            if (confidence.HasValue || qa.HasValue)
            {
                _conn.Perform(conn =>
                {
                    var from = conn.From<Comment>().Where(x => x.Id == commentId);

                    var sets = new List<string>();

                    if (confidence.HasValue)
                        sets.Add("sort_confidence = " + confidence.Value);

                    if (qa.HasValue)
                        sets.Add("sort_qa = " + qa.Value);

                    var statement = string.Format("update {0} set {1} {2}",
                            OrmLiteConfig.DialectProvider.GetQuotedTableName(ModelDefinition<Comment>.Definition),
                            string.Join(",", sets), 
                            from.WhereExpression);

                    conn.ExecuteNonQuery(statement);
                });
            }
        }

        public void DeleteComment(Guid commentId, DateTime deletedOn)
        {
            _conn.Perform(conn =>
            {
                conn.Update<Comment>(new
                    {
                        Deleted = true,
                        AuthorUserName = "deleted",
                        Body = "deleted on " + deletedOn.ToLongTimeString(),
                        BodyFormatted = "deleted"
                    },
                    x => x.Id == commentId);
            });
        }

        public void UpdateNumberOfReportsForComment(Guid commentId, int numberOfReports)
        {
            _conn.Perform(conn => conn.Update<Comment>(new { NumberOfReports = numberOfReports }, x => x.Id == commentId));
        }

        public void SetIgnoreReportsForComment(Guid commentId, bool ignoreReports)
        {
            _conn.Perform(conn => conn.Update<Comment>(new { IgnoreReports = ignoreReports }, x => x.Id == commentId));
        }

        public SeekedList<Guid> GetReportedComments(List<Guid> subs = null, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Comment>();

                if (subs != null && subs.Count > 0)
                    query.Where(x => subs.Contains(x.SubId));

                query.Where(x => x.NumberOfReports > 0);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                query.OrderByDescending(x => x.DateCreated);
                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public int GetNumberOfCommentsForPost(Guid postId)
        {
            return (int)_conn.Perform(conn => conn.Count<Comment>(x => x.PostId == postId));
        }

        public SeekedList<Guid> GetCommentsForUser(Guid userId, 
            CommentSortBy? sortBy = null, 
            int? skip = null,
            int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Comment>();

                query.Where(x => x.AuthorUserId == userId);

                if (sortBy.HasValue)
                {
                    switch (sortBy)
                    {
                        case CommentSortBy.Best:
                            query.OrderByDescending(x => x.SortConfidence);
                            break;
                        case CommentSortBy.Top:
                            query.OrderByExpression =
                                "ORDER BY (score(vote_up_count, vote_down_count), date_created) DESC";
                            break;
                        case CommentSortBy.New:
                            query.OrderByDescending(x => x.DateCreated);
                            break;
                        case CommentSortBy.Controversial:
                            query.OrderByExpression =
                                "ORDER BY (controversy(vote_up_count, vote_down_count), date_created) DESC";
                            break;
                        case CommentSortBy.Old:
                            query.OrderBy(x => x.DateCreated);
                            break;
                        case CommentSortBy.Qa:
                            query.OrderByDescending(x => x.SortQa);
                            break;
                        default:
                            throw new Exception("unknown sort");
                    }
                }
                else
                {
                    query.OrderByDescending(x => x.DateCreated);
                }

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }
    }
}
