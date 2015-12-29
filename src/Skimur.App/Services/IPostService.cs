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

        SeekedList<Guid> GetPosts(List<Guid> subs = null,
            PostsSortBy sortby = PostsSortBy.New,
            PostsTimeFilter timeFilter = PostsTimeFilter.All,
            Guid? userId = null,
            bool hideRemovedPosts = true,
            bool showDeleted = false,
            bool onlyAll = false,
            bool? nsfw = null,
            bool? sticky = null,
            bool stickyFirst = false,
            int? skip = null,
            int? take = null);

        SeekedList<Guid> QueryPosts(string text,
            Guid? subId = null,
            PostsSearchSortBy sortBy = PostsSearchSortBy.Relevance,
            PostsTimeFilter timeFilter = PostsTimeFilter.All,
            Guid? userId = null,
            bool hideRemovedPosts = true,
            bool showDeleted = false,
            bool? nsfw = null,
            int? skip = null,
            int? take = null);

        void UpdatePostVotes(Guid postId, int? upVotes, int? downVotes);

        SeekedList<Guid> GetUnmoderatedPosts(List<Guid> subs = null, int? skip = null, int? take = null);

        SeekedList<Guid> GetReportedPosts(List<Guid> subs = null, int? skip = null, int? take = null);

        void ApprovePost(Guid postId, Guid userId);

        void RemovePost(Guid postId, Guid userId);

        void UpdateNumberOfReportsForPost(Guid postId, int numberOfReports);

        void SetIgnoreReportsForPost(Guid postId, bool ignoreReports);

        void UpdateNumberOfCommentsForPost(Guid postId, int numberOfComments);

        void SetStickyForPost(Guid postId, bool sticky);

        void UpdateThumbnailForPost(Guid postId, string thumbnail);

        void UpdateMediaObjectForPost(Guid postId, Post.MediaObject mediaObject);
    }
}
