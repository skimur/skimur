using System.Collections.Generic;
using System.Drawing;

namespace Skimur.Scraper
{
    public interface IMediaScrapper
    {
        List<string> ExtractLinksFromHtml(string html);

        Image GetThumbnailForUrl(string url);

        Dictionary<string, string> ExtractOpenGraphData(string html);

        List<string> GetImagesFromHtml(string html);
    }
}
