using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Skimur.Messaging;
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
        private static Guid? _announcementSubId;

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
        public ActionResult Edit(EditPostModel model)
        {
            var response = _commandBus.Send<EditPostContent, EditPostContentResponse>(new EditPostContent
            {
                EditedBy = _userContext.CurrentUser.Id,
                PostId = model.PostId,
                Content = model.Content
            });

            if (!string.IsNullOrEmpty(response.Error))
                return CommonJsonResult(response.Error);

            var html = RenderView("_Post",_postWrapper.Wrap(model.PostId, _userContext.CurrentUser));

            return Json(new
            {
                success = true,
                postId = model.PostId,
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

        [SkimurAuthorize, Ajax]
        public ActionResult ToggleNsfw(Guid postId, bool nsfw)
        {
            _commandBus.Send(new TogglePostNsfw
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
                IsNsfw = nsfw
            });

            return CommonJsonResult(true);
        }

        [SkimurAuthorize, Ajax]
        public ActionResult ToggleSticky(Guid postId, bool sticky)
        {
            var response = _commandBus.Send<ToggleSticky, ToggleStickyResponse>(new ToggleSticky
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
                Sticky = sticky
            });

            return CommonJsonResult(response.Error);
        }

        [ChildActionOnly]
        public ActionResult AnnouncementPosts()
        {
            if (!_announcementSubId.HasValue)
            {
                var announcementSub = _subDao.GetSubByName("announcements");
                _announcementSubId = announcementSub != null ? announcementSub.Id : Guid.Empty;
            }

            if (_announcementSubId == Guid.Empty)
                return Content("");

            var sticky = _postDao.GetPosts(new List<Guid> {_announcementSubId.Value}, sticky: true);

            var posts = _postWrapper.Wrap(sticky, _userContext.CurrentUser);

            return PartialView("PostList", posts);
        }
    }
}
