using Infrastructure.Data;
using ServiceStack.OrmLite;

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
    }
}
