using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("SubScriptions")]
    public class SubScription : IAggregateRoot
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }
    }
}
