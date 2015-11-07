using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure;
using Skimur.Web.Mvc;
using Skimur.Web.Services;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    public class ThumbnailController : BaseController
    {
        private readonly IThumbnailCacheService _thumbnailCacheService;

        public ThumbnailController(IThumbnailCacheService thumbnailCacheService)
        {
            _thumbnailCacheService = thumbnailCacheService;
        }

        [LastModifiedCache]
        public ActionResult Thumbnail(string thumbnail, ThumbnailType type)
        {
            return File(_thumbnailCacheService.GetThumbnail(thumbnail, type), System.Web.MimeMapping.GetMimeMapping(thumbnail));
        }
    }
}
