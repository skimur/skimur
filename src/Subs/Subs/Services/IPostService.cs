using System;
using System.Collections.Generic;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface IPostService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        Post GetPostById(Guid id);

        SeekedList<Guid> GetPosts(List<Guid> subs = null, PostsSortBy sortby = PostsSortBy.New, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);

        SeekedList<Guid> QueryPosts(string text, Guid? subId = null, PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance, TimeFilter timeFilter = TimeFilter.All, int? skip = null, int? take = null);

        void UpdatePostVotes(Guid postId, int? upVotes, int? downVotes);
    }
}
