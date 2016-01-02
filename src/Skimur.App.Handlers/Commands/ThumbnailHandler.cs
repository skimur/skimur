using System;
using System.Drawing;
using Skimur.App.Commands;
using Skimur.App.Services;
using Skimur.Logging;
using Skimur.Messaging.Handling;
using Skimur.Scraper;

namespace Skimur.App.Handlers.Commands
{
    public class ThumbnailHandler : ICommandHandler<GenerateThumbnailForPost>
    {
        private readonly IPostService _postService;
        private readonly ILogger<ThumbnailHandler> _logger;
        private readonly IMediaScrapper _mediaScraper;
        private readonly IPostThumbnailService _postThumbnailService;

        public ThumbnailHandler(IPostService postService,
            ILogger<ThumbnailHandler> logger,
            IMediaScrapper mediaScraper,
            IPostThumbnailService postThumbnailService)
        {
            _postService = postService;
            _logger = logger;
            _mediaScraper = mediaScraper;
            _postThumbnailService = postThumbnailService;
        }

        public void Handle(GenerateThumbnailForPost command)
        {
            var post = _postService.GetPostById(command.PostId);
            if (post == null) return;
            
            if (!command.Force && !string.IsNullOrEmpty(post.Thumb))
                // already created and we aren't trying to for it to be recreated
                return;

            try
            {
                Image image = null;
                try
                {
                    if (post.PostType == PostType.Link)
                    {
                        // grab an image from the url of the post
                        image = _mediaScraper.GetThumbnailForUrl(post.Url);
                    }
                    else if (post.PostType == PostType.Text)
                    {
                        // this is a text post
                        // let's see if there are any links in the users post.
                        var links = _mediaScraper.ExtractLinksFromHtml(post.ContentFormatted);
                        if (links.Count > 0)
                        {

                        }

                        // TODO
                    }

                    if (image != null)
                    {
                        // we found an image for the link
                        _postService.UpdateThumbnailForPost(post.Id, _postThumbnailService.UploadImage(image));
                    }
                }
                finally
                {
                    if(image != null)
                        image.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("There was an error trying to get a thumbnail for the post " + post.Id, ex);
            }
        }
    }
}
