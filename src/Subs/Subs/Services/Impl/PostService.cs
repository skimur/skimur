using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Infrastructure.Data;
using ServiceStack.OrmLite;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services.Impl
{
    public class PostService : IPostService
    {
        private readonly IDbConnectionProvider _conn;

        public PostService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void InsertPost(Post post)
        {
            _conn.Perform(conn => conn.Insert(post));
        }

        public void UpdatePost(Post post)
        {
            _conn.Perform(conn => conn.Update(post));
        }

        public Post GetPostById(Guid id)
        {
            return _conn.Perform(conn => conn.SingleById<Post>(id));
        }

        public SeekedList<Guid> GetPosts(List<Guid> subs = null,
            PostsSortBy sortby = PostsSortBy.New,
            PostsTimeFilter timeFilter = PostsTimeFilter.All,
            Guid? userId = null,
            bool hideRemovedPosts = true,
            bool showDeleted = false,
            bool onlyAll = false,
            bool? nsfw = null,
            int? skip = null,
            int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();
                if (subs != null && subs.Count > 0)
                {
                    query.Where(x => subs.Contains(x.SubId));
                }

                if (timeFilter != PostsTimeFilter.All)
                {
                    TimeSpan timeSpan;

                    switch (timeFilter)
                    {
                        case PostsTimeFilter.Hour:
                            timeSpan = TimeSpan.FromHours(1);
                            break;
                        case PostsTimeFilter.Day:
                            timeSpan = TimeSpan.FromDays(1);
                            break;
                        case PostsTimeFilter.Week:
                            timeSpan = TimeSpan.FromDays(7);
                            break;
                        case PostsTimeFilter.Month:
                            timeSpan = TimeSpan.FromDays(30);
                            break;
                        case PostsTimeFilter.Year:
                            timeSpan = TimeSpan.FromDays(365);
                            break;
                        default:
                            throw new Exception("unknown time filter");
                    }

                    var from = Common.CurrentTime() - timeSpan;

                    query.Where(x => x.DateCreated >= from);
                }

                if (hideRemovedPosts)
                    query.Where(x => x.Verdict != (int)Verdict.ModRemoved);

                if (!showDeleted)
                    query.Where(x => x.Deleted == false);

                if (onlyAll)
                    query.Where(x => x.InAll);

                if (nsfw.HasValue)
                    query.Where(x => x.Nsfw == nsfw.Value);

                if (userId.HasValue)
                    query.Where(x => x.UserId == userId.Value);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);

                switch (sortby)
                {
                    case PostsSortBy.Hot:
                        query.OrderByExpression = "ORDER BY (hot(vote_up_count, vote_down_count, date_created), date_created) DESC";
                        break;
                    case PostsSortBy.New:
                        query.OrderByDescending(x => x.DateCreated);
                        break;
                    case PostsSortBy.Rising:
                        throw new Exception("not implemented");
                    case PostsSortBy.Controversial:
                        query.OrderByExpression = "ORDER BY (controversy(vote_up_count, vote_down_count), date_created) DESC";
                        break;
                    case PostsSortBy.Top:
                        query.OrderByExpression = "ORDER BY (score(vote_up_count, vote_down_count), date_created) DESC";
                        break;
                    default:
                        throw new Exception("uknown sort");
                }

                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public SeekedList<Guid> QueryPosts(string text,
            Guid? subId = null,
            PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance,
            PostsTimeFilter timeFilter = PostsTimeFilter.All,
            Guid? userId = null,
            bool hideRemovedPosts = true,
            bool showDeleted = false,
            bool? nsfw = null,
            int? skip = null,
            int? take = null)
        {
            // this implemention will eventually store an index, such as solr.

            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();

                if (subId.HasValue)
                {
                    query.Where(x => x.SubId == subId);
                }

                if (!string.IsNullOrEmpty(text))
                {
                    query.Where(x => x.Title.Contains(text) || x.Content.Contains(text));
                }

                if (timeFilter != PostsTimeFilter.All)
                {
                    TimeSpan timeSpan;
                    switch (timeFilter)
                    {
                        case PostsTimeFilter.Hour:
                            timeSpan = TimeSpan.FromHours(1);
                            break;
                        case PostsTimeFilter.Day:
                            timeSpan = TimeSpan.FromDays(1);
                            break;
                        case PostsTimeFilter.Week:
                            timeSpan = TimeSpan.FromDays(7);
                            break;
                        case PostsTimeFilter.Month:
                            timeSpan = TimeSpan.FromDays(30);
                            break;
                        case PostsTimeFilter.Year:
                            timeSpan = TimeSpan.FromDays(365);
                            break;
                        default:
                            throw new Exception("unknown time filter");
                    }

                    var from = Common.CurrentTime() - timeSpan;

                    query.Where(x => x.DateCreated >= from);
                }

                if (hideRemovedPosts)
                    query.Where(x => x.Verdict != (int)Verdict.ModRemoved);

                if (!showDeleted)
                    query.Where(x => x.Deleted == false);

                if (nsfw.HasValue)
                    query.Where(x => x.Nsfw == nsfw.Value);
                
                if (userId.HasValue)
                    query.Where(x => x.UserId == userId.Value);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);

                switch (sortBy)
                {
                    case PostsSearchSortBy.Relevance:
                        // let the db do its thing
                        break;
                    case PostsSearchSortBy.Top:
                        query.OrderByExpression = "ORDER BY (score(vote_up_count, vote_down_count), date_created) DESC";
                        break;
                    case PostsSearchSortBy.New:
                        query.OrderByDescending(x => x.DateCreated);
                        break;
                    case PostsSearchSortBy.Comments:
                        query.OrderByDescending(x => x.NumberOfComments);
                        break;
                    default:
                        throw new Exception("unknown sort");
                }

                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public void UpdatePostVotes(Guid postId, int? upVotes, int? downVotes)
        {
            if (downVotes.HasValue || upVotes.HasValue)
            {
                _conn.Perform(conn =>
                {
                    if (upVotes.HasValue && downVotes.HasValue)
                    {
                        conn.Update<Post>(new { VoteUpCount = upVotes.Value, VoteDownCount = downVotes.Value }, x => x.Id == postId);
                    }
                    else if (upVotes.HasValue)
                    {
                        conn.Update<Post>(new { VoteUpCount = upVotes.Value }, x => x.Id == postId);
                    }
                    else if (downVotes.HasValue)
                    {
                        conn.Update<Post>(new { VoteDownCount = downVotes.Value }, x => x.Id == postId);
                    }
                });
            }
        }

        public SeekedList<Guid> GetUnmoderatedPosts(List<Guid> subs = null, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();

                if (subs != null && subs.Count > 0)
                    query.Where(x => subs.Contains(x.SubId));

                query.Where(x => x.Verdict == (int)Verdict.None);

                query.Where(x => x.Deleted == false);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                query.OrderByDescending(x => x.DateCreated);
                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public SeekedList<Guid> GetReportedPosts(List<Guid> subs = null, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();

                if (subs != null && subs.Count > 0)
                    query.Where(x => subs.Contains(x.SubId));

                query.Where(x => x.NumberOfReports > 0);

                query.Where(x => x.Deleted == false);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                query.OrderByDescending(x => x.DateCreated);
                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }
        public void ApprovePost(Guid postId, Guid userId)
        {
            _conn.Perform(conn =>
            {
                conn.Update<Post>(new { ApprovedBy = userId, Verdict = (int)Verdict.ModApproved }, x => x.Id == postId);
            });
        }

        public void RemovePost(Guid postId, Guid userId)
        {
            _conn.Perform(conn =>
            {
                conn.Update<Post>(new { RemovedBy = userId, Verdict = (int)Verdict.ModRemoved }, x => x.Id == postId);
            });
        }

        public void UpdateNumberOfReportsForPost(Guid postId, int numberOfReports)
        {
            _conn.Perform(conn => conn.Update<Post>(new { NumberOfReports = numberOfReports }, x => x.Id == postId));
        }

        public void SetIgnoreReportsForPost(Guid postId, bool ignoreReports)
        {
            _conn.Perform(conn => conn.Update<Post>(new { IgnoreReports = ignoreReports }, x => x.Id == postId));
        }

        public void UpdateNumberOfCommentsForPost(Guid postId, int numberOfComments)
        {
            _conn.Perform(conn =>
            {
                conn.Update<Post>(new {NumberOfComments = numberOfComments}, x => x.Id == postId);
            });
        }
    }
}
