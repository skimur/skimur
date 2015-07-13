using System.Collections.Generic;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface IPostService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        Post GetPostBySlug(string slug);

        List<Post> GetPosts(List<string> subs = null, PostsSortBy sortby = PostsSortBy.Hot, TimeFilter timeFilter = TimeFilter.All);

        void UpdatePostVotes(string postSlug, int? upVotes, int? downVotes);
    }
}
