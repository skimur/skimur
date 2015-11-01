using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    public class ThumbnailController : BaseController
    {
        private readonly IPostThumbnailService _postThumbnailService;

        public ThumbnailController(IPostThumbnailService postThumbnailService)
        {
            _postThumbnailService = postThumbnailService;
        }

        public ActionResult Thumbnail(string thumbnail)
        {
            return File(_postThumbnailService.GetImage(thumbnail), System.Web.MimeMapping.GetMimeMapping(thumbnail));
        }
    }
}
