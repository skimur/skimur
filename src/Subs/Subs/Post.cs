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

        public string Title { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        public string Domain { get; set; }

        public bool SendReplies { get; set; }

        public int VoteUpCount { get; set; }

        public int VoteDownCount { get; set; }

        [Ignore]
        public PostType PostType
        {
            get { return (PostType) Type; }
            set { Type = (int)value; }
        }
    }

    public enum PostType
    {
        Link,
        Text
    }
}
