using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Skimur.Scraper
{
    public class MediaScrapper : IMediaScrapper
    {
        public List<string> ExtractLinksFromHtml(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.SelectNodes("//a").Select(x =>
            {
                var attribute = x.Attributes["href"];
                if (attribute == null) return null;
                return attribute.Value;
            }).Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        public Image GetThumbnailForUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var uri = new Uri(url);
            var uriBase = new Uri(uri.GetLeftPart(UriPartial.Authority));

            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            string html = null;
            using (var response = webRequest.GetResponse())
                if (!string.IsNullOrEmpty(response.ContentType))
                {
                    // if the response indicates this is an image, than that is what we use as a thumbnail
                    if (response.ContentType != null && response.ContentType.ToLower().StartsWith("image"))
                        using (var responseStream = response.GetResponseStream())
                            return Image.FromStream(responseStream);

                    if (response.ContentType.ToLower().StartsWith("text/html"))
                        using (var responseStream = response.GetResponseStream())
                            using (var streamReader = new StreamReader(responseStream))
                                html = streamReader.ReadToEnd();
                }

            if (!string.IsNullOrEmpty(html))
            {
                var openGraphData = ExtractOpenGraphData(html);
                if (openGraphData.ContainsKey("image"))
                {
                    return FromUrl(WebUtility.HtmlDecode(openGraphData["image"]));
                }
                if (openGraphData.ContainsKey("image:url"))
                {
                    return FromUrl(WebUtility.HtmlDecode(openGraphData["image"]));
                }

                // no clear indication about thumbnail, let's try to guess
                var images = GetImagesFromHtml(html)
                    // ensure relative urls have the appropriate protocol
                    .Select(x => x.StartsWith("//") ? uri.Scheme + x : x)
                    // ensure relative images are absolute
                    .Select(x =>
                    {
                        if (!x.StartsWith("http"))
                        {
                            return new Uri(uriBase, x).ToString();
                        }
                        return x;
                    })
                    // remove duplicate images
                    .Distinct()
                    .ToList();

                Image maxImage = null;
                int maxArea = 0;
                foreach (var imageUrl in images)
                {
                    Image image = null;
                    try
                    {
                        image = FromUrl(imageUrl);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (image == null) continue;

                    if (image.Width == 0 || image.Height == 0)
                    {
                        image.Dispose();
                        continue;
                    }

                    var area = image.Width*image.Height;

                    // ignore small images
                    if (area < 5000)
                    {
                        image.Dispose();
                        continue;
                    }

                    // ignore images that are really long/tall
                    if ((Math.Max(image.Width, image.Height)/(double)Math.Min(image.Width, image.Height) > 1.5))
                    {
                        image.Dispose();
                        continue;
                    }

                    if (imageUrl.ToLower().Contains("sprit"))
                    {
                        area /= 10;
                    }

                    if (area > maxArea)
                    {
                        maxArea = area;
                        if(maxImage != null)
                            maxImage.Dispose();
                        maxImage = image;
                    }
                }

                return maxImage;
            }
            return null;
        }

        public Dictionary<string, string> ExtractOpenGraphData(string html)
        {
            var indexOfClosingHead = Regex.Match(html, "</head>").Index;
            var toParse = html.Substring(0, indexOfClosingHead + 7);
            toParse = toParse + "<body></body></html>\r\n";

            var document = new HtmlDocument();
            document.LoadHtml(toParse);

            var allMeta = document.DocumentNode.SelectNodes("//meta");

            var openGraphMetaTags = from meta in allMeta ?? new HtmlNodeCollection(null)
                                    where (meta.Attributes.Contains("property") && meta.Attributes["property"].Value.StartsWith("og:")) ||
                                    (meta.Attributes.Contains("name") && meta.Attributes["name"].Value.StartsWith("og:"))
                                    select meta;

            var result = new Dictionary<string, string>();

            foreach (var metaTag in openGraphMetaTags)
            {
                var value = GetOpenGraphValue(metaTag);
                var property = GetOpenGraphKey(metaTag);

                if (string.IsNullOrWhiteSpace(value)) continue;
                if (result.ContainsKey(property)) continue;

                result.Add(property, value);
            }

            return result;
        }

        public List<string> GetImagesFromHtml(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.SelectNodes("//img").Select(x =>
            {
                var attribute = x.Attributes["src"];
                if (attribute == null) return null;
                return attribute.Value;
            }).Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        private string GetOpenGraphKey(HtmlNode metaTag)
        {
            return CleanOpenGraphKey(metaTag.Attributes.Contains("property") ? metaTag.Attributes["property"].Value : metaTag.Attributes["name"].Value);
        }

        private string GetOpenGraphValue(HtmlNode metaTag)
        {
            return !metaTag.Attributes.Contains("content") ? string.Empty : metaTag.Attributes["content"].Value;
        }

        private string CleanOpenGraphKey(string value)
        {
            return value.Replace("og:", string.Empty).ToLower(CultureInfo.InvariantCulture);
        }

        private Image FromUrl(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                    return Image.FromStream(responseStream);
        }
    }
}
