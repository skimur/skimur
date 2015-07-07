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

        public string UserName { get; set; }

        public string SubName { get; set; }

        public string AddedBy { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
