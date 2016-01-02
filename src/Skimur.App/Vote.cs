using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
{
    [Alias("Votes")]
    public class Vote
    {
        public virtual Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid UserId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public int Type { get; set; }

        public DateTime DateCasted { get; set; }

        public string IpAddress { get; set; }

        [Ignore]
        public VoteType VoteType
        {
            get { return (VoteType)Type; }
            set { Type = (int)value; }
        }
    }

    public enum VoteType
    {
        Up = 1,
        Down = 0
    }
}
