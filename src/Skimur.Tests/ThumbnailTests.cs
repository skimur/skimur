using NUnit.Framework;
using Skimur.Scraper;

namespace Skimur.Tests
{
    [TestFixture]
    public class ThumbnailTests
    {
        private IMediaScrapper _mediaScrapper;

        [Test]
        public void Can_extract_urls_from_html()
        {
            // arrange
            var html = string.Empty;
            html += "<a href=\"http://test1.com\">test1</a>";
            html += "<a href=\"\">test2</a>";
            html += "<a>test3</a>";
            html += "<a href='http://test4.com\'>test4</a>";
            html += "<a href='http://test5.com\'></a>";

            // act
            var urls = _mediaScrapper.ExtractLinksFromHtml(html);

            // assert
            Assert.That(urls, Has.Count.EqualTo(3));
            Assert.That(urls[0], Is.EqualTo("http://test1.com"));
            Assert.That(urls[1], Is.EqualTo("http://test4.com"));
            Assert.That(urls[2], Is.EqualTo("http://test5.com"));
        }

        [Test]
        public void Can_get_image_from_url()
        {
            // arrange/act
            var image = _mediaScrapper.GetThumbnailForUrl("https://static.skimur.com/content/img/logo.png");

            // assert
            Assert.That(image, Is.Not.Null);

            // arrange/act
            image = _mediaScrapper.GetThumbnailForUrl("https://www.reddit.com/r/funny/comments/3qsdnq/join_the_dork_side/");

            // assert
            Assert.That(image, Is.Not.Null);
        }

        [Test]
        public void Can_get_open_graph_data_from_html()
        {
            // arrange
            var html = "<html prefix=\"og: http://ogp.me/ns#\">" +
                       "<head>" +
                       "<title>The Rock (1996)</title>" +
                       "<meta property=\"og:title\" content=\"The Rock\" />" +
                       "<meta property=\"og:type\" content=\"video.movie\" />" +
                       "<meta property=\"og:url\" content=\"http://www.imdb.com/title/tt0117500/\" />" +
                       "<meta property=\"og:image\" content=\"http://ia.media-imdb.com/images/rock.jpg\" />" +
                       "</head>" +
                       "<div>test</div>" +
                       "</html>";

            // act
            var result = _mediaScrapper.ExtractOpenGraphData(html);

            // assert
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result["title"], Is.EqualTo("The Rock"));
            Assert.That(result["type"], Is.EqualTo("video.movie"));
            Assert.That(result["url"], Is.EqualTo("http://www.imdb.com/title/tt0117500/"));
            Assert.That(result["image"], Is.EqualTo("http://ia.media-imdb.com/images/rock.jpg"));
        }

        [Test]
        public void Can_get_images_from_html()
        {
            // arrange
            var html = "<img src='http://test.com/image1.jpg' />" +
                       "<img src=\"http://test.com/image2.jpg\" />" +
                       "<img src=\"\" />";

            // act
            var result = _mediaScrapper.GetImagesFromHtml(html);

            // assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("http://test.com/image1.jpg"));
            Assert.That(result[1], Is.EqualTo("http://test.com/image2.jpg"));
        }
        
        [SetUp]
        public void Setup()
        {
            _mediaScrapper = new MediaScrapper();
        }
    }
}
