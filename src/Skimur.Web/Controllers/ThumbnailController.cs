using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.StaticFiles;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNet.Http;

namespace Skimur.Web.Controllers
{
    public class ThumbnailController : BaseController
    {
        private readonly IThumbnailCacheService _thumbnailCacheService;
        private readonly IContentTypeProvider _contentTypeProvider;

        public ThumbnailController(IThumbnailCacheService thumbnailCacheService, 
            IContentTypeProvider contentTypeProvider)
        {
            _thumbnailCacheService = thumbnailCacheService;
            _contentTypeProvider = contentTypeProvider;
        }
        
        public ActionResult Thumbnail(string thumbnail, ThumbnailType type)
        {
            var responseHeaders = Response.GetTypedHeaders();
            responseHeaders.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromDays(300)
            };

            var file = _thumbnailCacheService.GetThumbnail(thumbnail, type);

            string contentType = null;
            if (!_contentTypeProvider.TryGetContentType(file, out contentType))
                throw new Exception("Can't get content type for " + file);
           
            return PhysicalFile(file, contentType);
        }
    }
}
