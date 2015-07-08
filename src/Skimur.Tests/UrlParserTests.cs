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
            AssertValidUrl("http://google.com/", "google.com", "http");
            AssertValidUrl("http://www.google.com/", "google.com", "http");
            AssertValidUrl("http://sub.google.com/", "sub.google.com", "http");
            AssertValidUrl("google.com", "google.com", "http");
            AssertValidUrl("www.google.com", "google.com", "http");
            AssertValidUrl("sub.google.com", "sub.google.com", "http");
            AssertValidUrl("google.com/test", "google.com", "http");
            AssertValidUrl("google.com/test?query=true", "google.com", "http");
            AssertValidUrl("google.com/test#hash?query=true", "google.com", "http");

            AssertInvalidUrl("google");
        }

        private void AssertValidUrl(string url, string domain, string scheme)
        {
            string domainResult;
            string schemeResult;
            Assert.That(UrlParser.TryParseUrl(url, out domainResult, out schemeResult), Is.True);
            Assert.That(domainResult, Is.EqualTo(domain));
            Assert.That(schemeResult, Is.EqualTo(scheme));
        }

        private void AssertInvalidUrl(string url)
        {
            string domain;
            string scheme;
            Assert.That(UrlParser.TryParseUrl(url, out domain, out scheme), Is.False, "domain=" + domain + ";scheme=" + scheme);
        }
    }
}
