using System;
using System.Collections.Generic;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class CreateEditSubModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string SidebarText { get; set; }

        public bool IsEditing { get; set; }
    }

    public class SubModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSubscribed { get; set; }

        public ulong NumberOfSubscribers { get; set; }
    }

    public class SubPostsModel
    {
        public SubModel Sub { get; set; }

        public PagedList<PostModel> Posts { get; set; }

        public PostsSortBy SortBy { get; set; }

        public TimeFilter? TimeFilter { get; set; }

        public bool IsFrontpage { get; set; }
    }

    public class SearchResultsModel
    {
        public string Query { get; set; }

        public SubModel LimitingToSub { get; set; }

        public PagedList<SubModel> Subs { get; set; }

        public PagedList<PostModel> Posts { get; set; }

        public PostsSearchSortBy SortBy { get; set; }

        public TimeFilter? TimeFilter { get; set; }

        public SearchResultType? ResultType { get; set; }
    }

    public enum SearchResultType
    {
        Sub,
        Post
    }
}
