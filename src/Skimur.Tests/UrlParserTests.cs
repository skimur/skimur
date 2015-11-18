using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Skimur.Tests
{
    [TestFixture]
    public class UrlParserTests
    {
        [Test]
        public void Can_parse_urls()
        {
            AssertValidUrl("http://google.com/", "http://google.com/", "google.com", "http");
            AssertValidUrl("http://www.google.com/", "http://www.google.com/", "google.com", "http");
            AssertValidUrl("http://sub.google.com/", "http://sub.google.com/", "sub.google.com", "http");
            AssertValidUrl("google.com", "http://google.com/", "google.com", "http");
            AssertValidUrl("www.google.com", "http://www.google.com/", "google.com", "http");
            AssertValidUrl("sub.google.com", "http://sub.google.com/", "sub.google.com", "http");
            AssertValidUrl("google.com/test", "http://google.com/test", "google.com", "http");
            AssertValidUrl("google.com/test?query=true", "http://google.com/test?query=true", "google.com", "http");
            AssertValidUrl("google.com/test#hash?query=true", "http://google.com/test#hash?query=true", "google.com", "http");
            AssertValidUrl("http://google.com/test/#.q9sk98j:KZ2b", "http://google.com/test/#.q9sk98j:KZ2b", "google.com", "http");

            // make sure schemes and domains are lower case at all times
            AssertValidUrl("HTtp://GooGle.CoM/ThiSIsaTesT", "http://google.com/ThiSIsaTesT", "google.com", "http");

            AssertInvalidUrl("google");
        }

        private void AssertValidUrl(string url, string formattedUrl, string domain, string scheme)
        {
            string domainResult;
            string schemeResult;
            string formattedUrlResult;
            Assert.That(UrlParser.TryParseUrl(url, out formattedUrlResult, out domainResult, out schemeResult), Is.True);
            Assert.That(domainResult, Is.EqualTo(domain));
            Assert.That(schemeResult, Is.EqualTo(scheme));
            Assert.That(formattedUrlResult, Is.EqualTo(formattedUrl));
        }

        private void AssertInvalidUrl(string url)
        {
            string domain;
            string scheme;
            string formattedUrl;
            Assert.That(UrlParser.TryParseUrl(url, out formattedUrl, out domain, out scheme), Is.False, "domain=" + domain + ";scheme=" + scheme);
        }
    }
}
