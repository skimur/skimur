using Microsoft.AspNet.Mvc;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Controllers
{
    public class AvatarController : BaseController
    {
        private readonly IAvatarService _avatarService;

        public AvatarController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        public ActionResult Key(string key)
        {
            var avatarStream = _avatarService.GetAvatarStream(key);

            if (avatarStream != null)
                return File(avatarStream, "image/jpeg");

            throw new NotFoundException();
        }
    }
}
