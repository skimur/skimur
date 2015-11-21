using System;
using System.Text;
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

        public static string RemoveBOM(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Trim('\uFEFF');
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

        public static bool IsReservedKeyword(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (value.Equals("CON", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("PRN", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("CLOCK$", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("CLOCK", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("NUL", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("PRN", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM0", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM1", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM2", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM3", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM4", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM5", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM6", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM7", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM8", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("COM9", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT0", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT1", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT2", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT3", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT4", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT5", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT6", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT7", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT8", StringComparison.InvariantCultureIgnoreCase)
                || value.Equals("LPT9", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
