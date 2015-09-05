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

        [Authorize]
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

        public ActionResult Approve(Guid postId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to approve a post."
                });
            }

            ApprovePostResponse response;

            try
            {
                response = _commandBus.Send<ApprovePost, ApprovePostResponse>(new ApprovePost
                {
                    PostId = postId,
                    ApprovedBy = _userContext.CurrentUser.Id
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error approving post.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            if (!string.IsNullOrEmpty(response.Error))
            {
                return Json(new
                {
                    success = false,
                    error = response.Error
                });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    error = (string)null
                });
            }
        }

        public ActionResult Remove(Guid postId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to remove a post."
                });
            }

            RemovePostResponse response;

            try
            {
                response = _commandBus.Send<RemovePost, RemovePostResponse>(new RemovePost
                {
                    PostId = postId,
                    RemovedBy = _userContext.CurrentUser.Id
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error removing post.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            if (!string.IsNullOrEmpty(response.Error))
            {
                return Json(new
                {
                    success = false,
                    error = response.Error
                });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    error = (string)null
                });
            }
        }
    }
}
