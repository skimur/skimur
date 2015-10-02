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

        public PostsController(ISubDao subDao,
            ISubWrapper subWrapper,
            IPostDao postDao,
            IPostWrapper postWrapper,
            IUserContext userContext,
            ICommandBus commandBus)
        {
            _subDao = subDao;
            _subWrapper = subWrapper;
            _postDao = postDao;
            _postWrapper = postWrapper;
            _userContext = userContext;
            _commandBus = commandBus;
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

        [SkimurAuthorize, Ajax]
        public ActionResult Edit(Guid postId, string content)
        {
            var response = _commandBus.Send<EditPostContent, EditPostContentResponse>(new EditPostContent
            {
                EditedBy = _userContext.CurrentUser.Id,
                PostId = postId,
                Content = content
            });

            if (!string.IsNullOrEmpty(response.Error))
                return CommonJsonResult(response.Error);

            var html = RenderView("_Post",_postWrapper.Wrap(postId, _userContext.CurrentUser));

            return Json(new
            {
                success = true,
                postId,
                html
            });
        }

        [SkimurAuthorize, Ajax]
        public ActionResult Delete(Guid postId, string reason)
        {
            var response = _commandBus.Send<DeletePost, DeletePostResponse>(new DeletePost
            {
                PostId = postId,
                DeleteBy = _userContext.CurrentUser.Id,
                Reason = reason
            });

            return CommonJsonResult(response.Error);
        }
    }
}
