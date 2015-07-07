using System;
using ServiceStack.DataAnnotations;

namespace Infrastructure.Membership
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
