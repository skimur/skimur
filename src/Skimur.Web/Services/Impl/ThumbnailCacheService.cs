using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ImageResizer;
using Infrastructure;
using Infrastructure.Settings;
using Subs.Services;

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

            using (var image = _postThumbnailService.GetImage(thumbnail))
            {
                switch (type)
                {
                    case ThumbnailType.Full:
                        using (var file = File.OpenWrite(possible))
                            image.CopyTo(file);
                        break;
                    case ThumbnailType.Post:
                        ImageBuilder.Current.Build(new ImageJob()
                        {
                            Source = image,
                            DisposeSourceObject = false,
                            Dest = possible,
                            Instructions = new Instructions("width=70&height=70&mode=max")
                        });
                        break;
                    default:
                        throw new Exception("unknown type");
                }
            }

            return possible;
        }
    }
}
