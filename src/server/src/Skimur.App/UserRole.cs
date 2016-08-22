using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App
{
    public class UserRole
    {
        /// <summary>
        /// UserId for the user that is in the role
        /// 
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// RoleId for the role
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
