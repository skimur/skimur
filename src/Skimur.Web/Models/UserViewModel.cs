using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; }

        public bool IsModerator { get; set; }

        public List<string> ModeratingSubs { get; set; } 

        public int CommentKudos { get; set; }

        public int PostKudos { get; set; }

        public List<KudosDetailsModel> KudosDetails { get; set; } 

        public PagedList<CommentWrapped> Comments { get; set; }
        
        public PagedList<PostWrapped> Posts { get; set; }

        public SortByEnum SortBy { get; set; }

        public TimeFilterEnum TimeFilter { get; set; }

        public enum SortByEnum
        {
            New,
            Hot,
            Top,
            Controversial
        }

        public enum TimeFilterEnum
        {
            Hour,
            Day,
            Week,
            Month,
            Year,
            All,
        }

        public class KudosDetailsModel
        {
            public string SubName { get; set; }

            public int PostKudos { get; set; }

            public int CommentKudos { get; set; }

            public int Total { get { return PostKudos + CommentKudos; } }
        }
    }
}
