using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("Subs")]
    public class Sub : IAggregateRoot
    {
        public virtual Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string SidebarText { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public int NumberOfSubscribers { get; set; }
    }
}
