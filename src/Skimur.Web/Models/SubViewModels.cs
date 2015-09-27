using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class CreateEditSubModel
    {
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Sidebar text")]
        public string SidebarText { get; set; }

        [DisplayName("Type")]
        public SubType SubType { get; set; }

        [DisplayName("Is default")]
        public bool? IsDefault { get; set; }
        
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

        public string Query { get; set; }

        public BanUserModel BanUser { get; set; }
    }

    public class BanUserModel
    {
        [Required]
        public string UserName { get; set; }
        
        public string ReasonPrivate { get; set; }

        public string ReasonPublic { get; set; }
    }
}
