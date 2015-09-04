using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Models;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class PostsController : BaseController
    {
        private readonly ISubDao _subDao;
        private readonly ISubWrapper _subWrapper;
        private readonly IPostDao _postDao;
        private readonly IPostWrapper _postWrapper;
        private readonly IUserContext _userContext;

        public PostsController(ISubDao subDao,
            ISubWrapper subWrapper,
            IPostDao postDao,
            IPostWrapper postWrapper,
            IUserContext userContext)
        {
            _subDao = subDao;
            _subWrapper = subWrapper;
            _postDao = postDao;
            _postWrapper = postWrapper;
            _userContext = userContext;
        }

        public ActionResult Unmoderated(string subName)
        {
            if (string.IsNullOrEmpty(subName))
                throw new NotFoundException();

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                throw new NotFoundException();

            var postIds = _postDao.GetUnmoderatedPosts(new List<Guid> { sub.Id }, take: 30);

            var model = new SubPostsModel();
            model.Sub = _subWrapper.Wrap(sub.Id, _userContext.CurrentUser);
            model.SortBy = PostsSortBy.New;
            model.Posts = new PagedList<PostWrapped>(_postWrapper.Wrap(postIds, _userContext.CurrentUser), 0, 30, postIds.HasMore);

            return View(model);
        }
    }
}
