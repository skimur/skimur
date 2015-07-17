using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface IPostDao
    {
        List<Post> GetPosts(List<string> subs = null, PostsSortBy sortBy = PostsSortBy.New, TimeFilter timeFilter = TimeFilter.All);

        List<Post> QueryPosts(string query, string sub = null, PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance, TimeFilter timeFilter = TimeFilter.All);
    }

    public enum PostsSearchSortBy
    {
        Relevance,
        Top,
        New,
        Comments
    }

    public enum PostsSortBy
    {
        Hot,
        New,
        Rising,
        Controversial,
        Top
    }

    public enum TimeFilter
    {
        All,
        Hour,
        Day,
        Week,
        Month,
        Year
    }
}
