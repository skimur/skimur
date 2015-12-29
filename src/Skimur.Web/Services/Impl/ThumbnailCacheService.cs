using ImageResizer;
using Skimur.Settings;
using Skimur.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App.Services;

namespace Skimur.Web.Services.Impl
{
    public class ThumbnailCacheService : IThumbnailCacheService
    {
        private readonly IPostThumbnailService _postThumbnailService;
        private string _thumbnailCacheDirectory;

        public ThumbnailCacheService(ISettingsProvider<WebSettings> settings,
            IPathResolver pathResolver,
            IPostThumbnailService postThumbnailService)
        {
            _postThumbnailService = postThumbnailService;
            _thumbnailCacheDirectory = pathResolver.Resolve(settings.Settings.ThumbnailCache);
            foreach (string type in Enum.GetValues(typeof(ThumbnailType)).Cast<ThumbnailType>().Select(x => x.ToString()))
            {
                var cacheDirectory = Path.Combine(_thumbnailCacheDirectory, type);
                if (!Directory.Exists(cacheDirectory))
                    Directory.CreateDirectory(cacheDirectory);
            }
        }

        public string GetThumbnail(string thumbnail, ThumbnailType type)
        {
            var possible = Path.Combine(_thumbnailCacheDirectory, type.ToString(), thumbnail);
            if (File.Exists(possible))
                return possible;

            // TODO: global sync lock?

            using (var imageStream = _postThumbnailService.GetImage(thumbnail))
            {
                switch (type)
                {
                    case ThumbnailType.Full:
                        using (var fileStream = File.OpenWrite(possible))
                            imageStream.CopyTo(fileStream);
                        break;
                    case ThumbnailType.Post:

                        using (var image = Image.FromStream(imageStream))
                        {
                            var instructions = new Instructions("width=70&height=70&scale=both")
                            {
                                Width = 70,
                                Height = 70,
                                Scale = ScaleMode.Both
                            };

                            // we want horizontally long images to have no padding. simply render the images with a max width.
                            // for vertically long images, we want to fill the entire canvas area with no black space on the sides.
                            var ratio = image.Width / (double)image.Height;
                            if (ratio >= 1.5)
                            {
                                // the image is REALLY wide, so, let's just fill, cropping the ends.
                                instructions.Mode = FitMode.Crop;
                            }
                            else if (ratio >= 1)
                            {
                                // the image is wide
                                instructions.Mode = FitMode.Max;
                            }
                            else
                            {
                                // the image is tall
                                instructions.Mode = FitMode.Crop;
                            }

                            ImageBuilder.Current.Build(new ImageJob()
                            {
                                Source = image,
                                Dest = possible,
                                Instructions = instructions
                            });
                        }
                        break;
                    default:
                        throw new Exception("unknown type");
                }
            }

            return possible;
        }
    }
}
