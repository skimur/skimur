using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skimur
{
    public class UrlParser
    {
        public static bool TryParseUrl(string url, out string domain, out string scheme)
        {
            domain = null;
            scheme = null;

            if (string.IsNullOrEmpty(url)) return false;

            url = url.ToLower();

            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }

            if (!Regex.IsMatch(url, @"^(http|https|):\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$"))
                return false;

            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                return false;
            }

            domain = uri.Host.StartsWith("www.") ? uri.Host.Substring(4) : uri.Host;
            scheme = uri.Scheme;

            return true;
        }
    }
}
