using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Skimur.Tests
{
    public class UrlParserTests
    {
        [Fact]
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
            UrlParser.TryParseUrl(url, out formattedUrlResult, out domainResult, out schemeResult).Should().BeTrue();
            domainResult.Should().Be(domain);
            schemeResult.Should().Be(scheme);
            formattedUrlResult.Should().Be(formattedUrl);
        }

        private void AssertInvalidUrl(string url)
        {
            string domain;
            string scheme;
            string formattedUrl;
            UrlParser.TryParseUrl(url, out formattedUrl, out domain, out scheme).Should().BeFalse();
        }
    }
}
