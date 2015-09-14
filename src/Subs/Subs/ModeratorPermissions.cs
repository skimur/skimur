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
        All = 1,
        Access = 1 << 1,
        Config = 1 << 2,
        Flair = 1 << 3,
        Mail = 1 << 4,
        Posts = 1 << 5
    }
}
