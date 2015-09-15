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
                return permissions.Value.HasFlag(permission) || permissions.Value.HasFlag(ModeratorPermissions.All);
            return permissions.Value.HasFlag(ModeratorPermissions.All);
        }

        public static bool HasPermission(this ModeratorPermissions permissions, ModeratorPermissions permission)
        {
            if (permission != ModeratorPermissions.All)
                return permissions.HasFlag(permission) || permissions.HasFlag(ModeratorPermissions.All);
            return permissions.HasFlag(ModeratorPermissions.All);
        }
    }
}
