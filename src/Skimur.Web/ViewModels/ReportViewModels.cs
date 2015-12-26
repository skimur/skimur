using Subs;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
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
