using System;

namespace Skimur.App
{
    [Flags]
    public enum ModeratorPermissions
    {
        None = 0,
        All = 1,
        Access = 1 << 1,
        Config = 1 << 2,
        Flair = 1 << 3,
        Mail = 1 << 4,
        Posts = 1 << 5,
        Styles = 1 << 6
    }
}
