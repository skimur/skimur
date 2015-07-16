using System;
using System.Dynamic;
using Infrastructure.Data;
using ServiceStack.OrmLite;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
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

        public Post GetPostBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return null;
            return _conn.Perform(conn => conn.Single<Post>(x => x.Slug == slug));
        }

        public System.Collections.Generic.List<Post> GetPosts(System.Collections.Generic.List<string> subs = null, PostsSortBy sortBy = PostsSortBy.Hot, TimeFilter timeFilter = TimeFilter.All)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Post>();
                if (subs != null && subs.Count > 0)
                {
                    query.Where(x => subs.Contains(x.SubName));
                }

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
                        break;
                    case PostsSortBy.Controversial:
                        query.OrderByExpression = "ORDER BY (controversy(vote_up_count, vote_down_count), date_created) DESC";
                        break;
                    case PostsSortBy.Top:
                        query.OrderByExpression = "ORDER BY (score(vote_up_count, vote_down_count), date_created) DESC";
                        break;
                    default:
                        throw new Exception("uknown sort");
                }

                //CREATE INDEX posts_hot_index ON posts (hot(vote_up_count, vote_down_count, date_created), date_created);
                //CREATE INDEX posts_score_index ON posts (score(vote_up_count, vote_down_count), date_created);
                //CREATE INDEX posts_controversy_index ON posts (controversy(vote_up_count, vote_down_count), date_created);

                if (timeFilter != TimeFilter.All)
                {
                    TimeSpan timeSpan;
                    switch (timeFilter)
                    {
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

                return conn.Select(query);
            });
        }

        public void UpdatePostVotes(string postSlug, int? upVotes, int? downVotes)
        {
            if (downVotes.HasValue || upVotes.HasValue)
            {
                _conn.Perform(conn =>
                {
                    var post = conn.Single<Post>(x => x.Slug.ToLower() == postSlug.ToLower());
                    if (post != null)
                    {
                        if (upVotes.HasValue && downVotes.HasValue)
                        {
                            conn.Update<Post>(new { VoteUpCount = upVotes.Value, VoteDownCount = downVotes.Value }, x => x.Id == post.Id);
                        }
                        else if (upVotes.HasValue)
                        {
                            conn.Update<Post>(new { VoteUpCount = upVotes.Value }, x => x.Id == post.Id);
                        }
                        else if (downVotes.HasValue)
                        {
                            conn.Update<Post>(new { VoteDownCount = downVotes.Value }, x => x.Id == post.Id);
                        }
                    }
                });
            }
        }
    }
}
