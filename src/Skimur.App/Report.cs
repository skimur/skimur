using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace Subs
{
    public class Report
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid ReportedBy { get; set; }
        
        public string Reason { get; set; }
        
        [Alias("ReportedPosts")]
        public class PostReport : Report
        {
            [Alias("Post")]
            public Guid PostId { get; set; }
        }

        [Alias("ReportedComments")]
        public class CommentReport : Report
        {
            [Alias("Comment")]
            public Guid CommentId { get; set; }
        }
    }
}
