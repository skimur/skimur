using System;
using System.Collections.Generic;
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

        public SeekedList<Guid> GetPosts(List<Guid> subs = null, PostsSortBy sortBy = PostsSortBy.Hot, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();
                if (subs != null && subs.Count > 0)
                {
                    query.Where(x => subs.Contains(x.SubId));
                }

                if (timeFilter != TimeFilter.All)
                {
                    TimeSpan timeSpan;
                    switch (timeFilter)
                    {
                        case TimeFilter.Hour:
                            timeSpan = TimeSpan.FromHours(1);
                            break;
                        case TimeFilter.Day:
                            timeSpan = TimeSpan.FromDays(1);
                            break;
                        case TimeFilter.Week:
                            timeSpan = TimeSpan.FromDays(7);
                            break;
                        case TimeFilter.Month:
                            timeSpan = TimeSpan.FromDays(30);
                            break;
                        case TimeFilter.Year:
                            timeSpan = TimeSpan.FromDays(365);
                            break;
                        default:
                            throw new Exception("unknown time filter");
                    }

                    var from = Common.CurrentTime() - timeSpan;

                    query.Where(x => x.DateCreated >= from);
                }

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);

                switch (sortBy)
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

        public SeekedList<Guid> QueryPosts(string text, Guid? subId = null, PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null)
        {
            // this implemention will eventually store a index, such as solr.

            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();

                if (subId.HasValue)
                {
                    query.Where(x => x.SubId == subId);
                }

                if(!string.IsNullOrEmpty(text))
                {
                    query.Where(x => x.Title.Contains(text) || x.Content.Contains(text));
                }

                if (timeFilter != TimeFilter.All)
                {
                    TimeSpan timeSpan;
                    switch (timeFilter)
                    {
                        case TimeFilter.Hour:
                            timeSpan = TimeSpan.FromHours(1);
                            break;
                        case TimeFilter.Day:
                            timeSpan = TimeSpan.FromDays(1);
                            break;
                        case TimeFilter.Week:
                            timeSpan = TimeSpan.FromDays(7);
                            break;
                        case TimeFilter.Month:
                            timeSpan = TimeSpan.FromDays(30);
                            break;
                        case TimeFilter.Year:
                            timeSpan = TimeSpan.FromDays(365);
                            break;
                        default:
                            throw new Exception("unknown time filter");
                    }

                    var from = Common.CurrentTime() - timeSpan;

                    query.Where(x => x.DateCreated >= from);
                }

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
                        // TODO:
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
    }
}
