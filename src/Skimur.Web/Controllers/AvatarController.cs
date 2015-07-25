using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Avatar;

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

            return File(Server.MapPath("~/Content/img/avatar.jpg"), "image/jpeg");
        }
    }
}
