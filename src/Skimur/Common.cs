using System;
using System.Text.RegularExpressions;

namespace Skimur
{
    public static class Common
    {
        public static Func<DateTime> CurrentTime = () => DateTime.UtcNow;

        public static string UrlFriendly(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Replace(" ", "_");

            value = Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "");

            return value;
        }
    }
}
