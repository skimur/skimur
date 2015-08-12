using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Membership;
using Subs;
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
    
    public class SubPostsModel
    {
        public SubWrapped Sub { get; set; }

        public PagedList<PostWrapped> Posts { get; set; }

        public PostsSortBy SortBy { get; set; }

        public TimeFilter? TimeFilter { get; set; }

        public bool IsFrontpage { get; set; }
    }

    public class SearchResultsModel
    {
        public string Query { get; set; }

        public SubWrapped LimitingToSub { get; set; }

        public PagedList<SubWrapped> Subs { get; set; }

        public PagedList<PostWrapped> Posts { get; set; }

        public PostsSearchSortBy SortBy { get; set; }

        public TimeFilter? TimeFilter { get; set; }

        public SearchResultType? ResultType { get; set; }
    }

    public enum SearchResultType
    {
        Sub,
        Post
    }

    public class BannedUsersFromSub
    {
        public Sub Sub { get; set; }

        public PagedList<SubUserBanWrapped> Users { get; set; }

        public BanUserModel BanUser { get; set; }
    }

    public class BanUserModel
    {
        [Required]
        public string UserName { get; set; }

        public DateTime? BannedUntil { get; set; }
        
        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }
}
