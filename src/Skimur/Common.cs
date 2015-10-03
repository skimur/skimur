using System;
using System.Text.RegularExpressions;

namespace Skimur
{
    public static class Common
    {
        public static Regex WhiteSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        public static Regex NotSafeRegex = new Regex(@"\W+", RegexOptions.Compiled);
        public static Regex UnderscoreRegex = new Regex(@"_+", RegexOptions.Compiled);
        public static Regex NsfwRegex = new Regex(@"\bnsf[wl]\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Func<DateTime> CurrentTime = () => DateTime.UtcNow;

        public static string UrlFriendly(this string value, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(value)) return "";

            value = WhiteSpaceRegex.Replace(value, "_");
            value = NotSafeRegex.Replace(value, "");
            value = UnderscoreRegex.Replace(value, "_");
            value = value.Trim('_');
            value = value.ToLower();

            if (value.Length > maxLength)
            {
                value = value.Substring(0, maxLength);
                var lastWord = value.LastIndexOf('_');
                if (lastWord > 0)
                    value = value.Substring(0, lastWord);
            }

            return value;
        }

        public static int Fuzz(int value)
        {
            var decay = Math.Exp((float)-value/60);
            var jitter = (int)Math.Round(5*decay);
            return value + new Random().Next(0, jitter);
        }

        public static bool IsNsfw(string text)
        {
            return !string.IsNullOrEmpty(text) && (!string.IsNullOrEmpty(text) && NsfwRegex.IsMatch(text));
        }
    }
}
