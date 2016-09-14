using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App
{
    [Alias("Roles")]
    public class Role
    {
        /// <summary>
        /// The unique id for the role
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// The role name
        /// </summary>
        public virtual string Name { get; set; }
    }
}
