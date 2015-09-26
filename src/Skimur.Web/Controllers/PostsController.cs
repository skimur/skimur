using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Skimur.Web.Mvc;
using Subs;
using Subs.Commands;
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
        private readonly ICommandBus _commandBus;
        private readonly ILogger<PostsController> _logger;

        public PostsController(ISubDao subDao,
            ISubWrapper subWrapper,
            IPostDao postDao,
            IPostWrapper postWrapper,
            IUserContext userContext,
            ICommandBus commandBus,
            ILogger<PostsController> logger)
        {
            _subDao = subDao;
            _subWrapper = subWrapper;
            _postDao = postDao;
            _postWrapper = postWrapper;
            _userContext = userContext;
            _commandBus = commandBus;
            _logger = logger;
        }

        [SkimurAuthorize]
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

        [SkimurAuthorize, Ajax]
        public ActionResult Approve(Guid postId)
        {
            var response = _commandBus.Send<ApprovePost, ApprovePostResponse>(new ApprovePost
                {
                    PostId = postId,
                    ApprovedBy = _userContext.CurrentUser.Id
                });
           
            return CommonJsonResult(response.Error);
        }

        [SkimurAuthorize, Ajax]
        public ActionResult Remove(Guid postId)
        {
            var response = _commandBus.Send<RemovePost, RemovePostResponse>(new RemovePost
                {
                    PostId = postId,
                    RemovedBy = _userContext.CurrentUser.Id
                });
           
            return CommonJsonResult(response.Error);
        }
    }
}
