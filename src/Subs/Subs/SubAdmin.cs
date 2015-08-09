using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("SubAdmins")]
    public class SubAdmin : IAggregateRoot
    {
        public virtual Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SubId { get; set; }

        public Guid? AddedBy { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
