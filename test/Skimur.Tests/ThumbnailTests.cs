using FluentAssertions;
using Skimur.Scraper;
using Xunit;

namespace Skimur.Tests
{
    public class ThumbnailTests
    {
        private IMediaScrapper _mediaScrapper;

        public ThumbnailTests()
        {
            _mediaScrapper = new MediaScrapper();
        }

        [Fact]
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
            urls.Count.Should().Be(3);
            urls[0].Should().Be("http://test1.com");
            urls[1].Should().Be("http://test4.com");
            urls[2].Should().Be("http://test5.com");
        }

        [Fact]
        public void Can_get_image_from_url()
        {
            // arrange/act
            var image = _mediaScrapper.GetThumbnailForUrl("https://static.skimur.com/content/img/logo.png");

            // assert
            image.Should().NotBeNull();

            // TODO: Gaurd against invalid urls. They shouldn't throw exceptions.
            //// arrange/act
            //image = _mediaScrapper.GetThumbnailForUrl("https://www.reddit.com/r/funny/comments/3qsdnq/join_the_dork_side/");

            //// assert
            //image.Should().NotBeNull();
        }

        [Fact]
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
            result.Count.Should().Be(4);
            result["title"].Should().Be("The Rock");
            result["type"].Should().Be("video.movie");
            result["url"].Should().Be("http://www.imdb.com/title/tt0117500/");
            result["image"].Should().Be("http://ia.media-imdb.com/images/rock.jpg");
        }

        [Fact]
        public void Can_get_images_from_html()
        {
            // arrange
            var html = "<img src='http://test.com/image1.jpg' />" +
                       "<img src=\"http://test.com/image2.jpg\" />" +
                       "<img src=\"\" />";

            // act
            var result = _mediaScrapper.GetImagesFromHtml(html);

            // assert
            result.Count.Should().Be(2);
            result[0].Should().Be("http://test.com/image1.jpg");
            result[1].Should().Be("http://test.com/image2.jpg");
        }
    }
}
