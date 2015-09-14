using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    public static class SubExtensions
    {
        public static bool HasPermission(this ModeratorPermissions? permissions, ModeratorPermissions permission)
        {
            if (!permissions.HasValue) return false;

            if (permission != ModeratorPermissions.All)
                return permissions.Value.HasFlag(permissions) || permissions.Value.HasFlag(ModeratorPermissions.All);
            return permission.HasFlag(ModeratorPermissions.All);
        }

        public static bool HasPermission(this ModeratorPermissions permissions, ModeratorPermissions permission)
        {
            if (permission != ModeratorPermissions.All)
                return permissions.HasFlag(permissions) || permissions.HasFlag(ModeratorPermissions.All);
            return permission.HasFlag(ModeratorPermissions.All);
        }
    }
}
