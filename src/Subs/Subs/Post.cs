using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("Posts")]
    public class Post : IAggregateRoot
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? LastEditDate { get; set; }

        public string Slug { get; set; }

        public Guid SubId { get; set; }

        public Guid UserId { get; set; }

        public string UserIp { get; set; }

        public int Type { get; set; }

        [Ignore]
        public PostType PostType
        {
            get { return (PostType)Type; }
            set { Type = (int)value; }
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ContentFormatted { get; set; }

        public string Url { get; set; }

        public string Domain { get; set; }

        public bool SendReplies { get; set; }

        public int VoteUpCount { get; set; }

        public int VoteDownCount { get; set; }

        public int Verdict { get; set; }

        public string VerdictMessage { get; set; }

        public Guid? ApprovedBy { get; set; }

        public Guid? RemovedBy { get; set; }
        
        public int NumberOfReports { get; set; }

        public bool IgnoreReports { get; set; }

        public int NumberOfComments { get; set; }

        public bool Deleted { get; set; }

        public bool Nsfw { get; set; }

        public string Mirrored { get; set; }

        public bool InAll { get; set; }

        public bool Sticky { get; set; }

        public string Thumb { get; set; }

        [Ignore]
        public Verdict PostVerdict
        {
            get { return (Verdict)Verdict; }
            set { Verdict = (int)value; }
        }
    }

    public enum PostType
    {
        Link,
        Text
    }

    public enum Verdict
    {
        None = 0,
        ModApproved = 1,
        ModRemoved = 2
    }
}
