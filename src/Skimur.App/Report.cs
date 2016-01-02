using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
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
