using System.Collections.Generic;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface IPostService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        Post GetPostBySlug(string slug);

        SeekedList<Post> GetPosts(List<string> subs = null, PostsSortBy sortby = PostsSortBy.New, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);

        SeekedList<Post> QueryPosts(string text, string sub = null, PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);

        void UpdatePostVotes(string postSlug, int? upVotes, int? downVotes);
    }
}
