using System.Collections.Generic;

namespace Subs.Services
{
    public interface IPostService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        Post GetPostBySlug(string slug);

        List<Post> GetPosts(List<string> subs = null);

        void UpdatePostVotes(string postSlug, int? upVotes, int? downVotes);
    }
}
