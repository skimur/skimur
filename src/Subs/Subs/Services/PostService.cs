using System.Dynamic;
using Infrastructure.Data;
using ServiceStack.OrmLite;
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

        public System.Collections.Generic.List<Post> GetPosts(System.Collections.Generic.List<string> subs = null, PostsSortBy sortBy = PostsSortBy.Hot)
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
                        case PostsSortBy.New:
                        break;
                    default:
                        break;
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
                        }else if (upVotes.HasValue)
                        {
                            conn.Update<Post>(new { VoteUpCount = upVotes.Value }, x => x.Id == post.Id);
                        }else if (downVotes.HasValue)
                        {
                            conn.Update<Post>(new { VoteDownCount = downVotes.Value }, x => x.Id == post.Id);
                        }
                    }
                });
            }
        }
    }
}
