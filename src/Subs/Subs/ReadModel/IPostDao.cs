using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;

namespace Subs.ReadModel
{
    public interface IPostDao
    {
        Post GetPostById(Guid id);

        SeekedList<Post> GetPosts(List<Guid> subs = null, PostsSortBy sortBy = PostsSortBy.New, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);

        SeekedList<Post> QueryPosts(string query, Guid? subId = null, PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);
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
