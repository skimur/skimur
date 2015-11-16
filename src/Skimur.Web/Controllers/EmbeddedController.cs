using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Models;
using Subs.ReadModel;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    public class EmbeddedController : BaseController
    {
        private readonly IPostDao _postDao;

        public EmbeddedController(IPostDao postDao)
        {
            _postDao = postDao;
        }

        public ActionResult Embedded(Guid postId)
        {
            var post = _postDao.GetPostById(postId);
            return View(new EmbeddedModel {Url = post.Url});
        }
    }
}
