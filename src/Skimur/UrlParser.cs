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
        public static bool TryParseUrl(string url, out string formattedUrl, out string domain, out string scheme)
        {
            domain = null;
            scheme = null;
            formattedUrl = null;

            if (string.IsNullOrEmpty(url)) return false;
            
            Uri uri;
            try
            {
                uri = PrepUrlRecursive(url);
            }
            catch (Exception)
            {
                return false;
            }

            if (string.IsNullOrEmpty(uri.Host)) return false;

            domain = uri.Host.ToLower();

            if (domain.StartsWith("www."))
                domain = domain.Substring(4);
            
            if (!domain.Contains("."))
                return false; // domain should have at least 1 period (.com, .io, etc)

            if (string.IsNullOrEmpty(uri.Scheme)) return false;

            scheme = uri.Scheme.ToLower();

            formattedUrl = uri.ToString();

            return true;
        }

        private static Uri PrepUrlRecursive(string url, int pass = 1)
        {
            if(pass >= 10) throw new Exception("Recursive error prepping url for inspection.");

            try
            {
                return new Uri(url);
            }
            catch (UriFormatException ex)
            {
                if (ex.Message == "Invalid URI: The format of the URI could not be determined.")
                    return PrepUrlRecursive("http://" + url, pass + 1);

                throw;
            }
        }
    }
}
