using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
{
    [Alias("SubScriptions")]
    public class SubScription
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }
    }
}
