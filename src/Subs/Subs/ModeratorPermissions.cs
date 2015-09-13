using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    [Flags]
    public enum ModeratorPermissions
    {
        Access = 1,
        Config = 1 << 1,
        Flair = 1 << 2,
        Mail = 1 << 3,
        Posts = 1 << 4,
        All = Access | Config | Flair | Mail | Posts
    }
}
