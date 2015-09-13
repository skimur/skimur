using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class ReportedPostsViewModel
    {
        public Sub Sub { get; set; }

        public PagedList<PostWrapped> Posts { get; set; }
    }

    public class ReportedCommentsViewModel
    {
        public Sub Sub { get; set; }

        public PagedList<CommentWrapped> Comments { get; set; } 
    }
}
