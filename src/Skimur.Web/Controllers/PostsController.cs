using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Skimur.Messaging;
using Skimur.Web.Models;
using Skimur.Web.Mvc;
using Subs;
using Subs.Commands;
using Subs.ReadModel;
using Subs.Services;

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
        private readonly IContextService _contextService;
        private readonly ICommentTreeContextBuilder _commentTreeContextBuilder;
        private readonly ICommentDao _commentDao;
        private readonly ISubActivityDao _subActivityDao;
        private readonly ICommentNodeHierarchyBuilder _commentNodeHierarchyBuilder;
        private static Guid? _announcementSubId;

        public PostsController(ISubDao subDao,
            ISubWrapper subWrapper,
            IPostDao postDao,
            IPostWrapper postWrapper,
            IUserContext userContext,
            ICommandBus commandBus,
            IContextService contextService,
            ICommentTreeContextBuilder commentTreeContextBuilder,
            ICommentDao commentDao,
            ISubActivityDao subActivityDao,
            ICommentNodeHierarchyBuilder commentNodeHierarchyBuilder)
        {
            _subDao = subDao;
            _subWrapper = subWrapper;
            _postDao = postDao;
            _postWrapper = postWrapper;
            _userContext = userContext;
            _commandBus = commandBus;
            _contextService = contextService;
            _commentTreeContextBuilder = commentTreeContextBuilder;
            _commentDao = commentDao;
            _subActivityDao = subActivityDao;
            _commentNodeHierarchyBuilder = commentNodeHierarchyBuilder;
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

        public ActionResult Frontpage(PostsSortBy? sort, PostsTimeFilter? time, int? pageNumber, int? pageSize)
        {
            var subs = _contextService.GetSubscribedSubIds();

            // if the user is not subscribe to any subs, show the default content.
            if (subs.Count == 0)
                subs = _subDao.GetDefaultSubs();

            if (sort == null)
                sort = PostsSortBy.Hot;

            if (time == null)
                time = PostsTimeFilter.All;

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var postIds = _postDao.GetPosts(subs,
                sortby: sort.Value,
                timeFilter: time.Value,
                // anonymous users don't see NSFW content.
                // logged in users only see NSFW if preferences say so.
                // If they want to see NSFW, they will see all content (SFW/NSFW).
                nsfw: _userContext.CurrentUser == null ? false : (_userContext.CurrentUser.ShowNsfw ? (bool?)null : false),
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            var model = new SubPostsModel();
            model.SortBy = sort.Value;
            model.TimeFilter = time;
            if (subs.Any()) // maybe the user hasn't subscribed to any subs?
                model.Posts = new PagedList<PostWrapped>(_postWrapper.Wrap(postIds, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, postIds.HasMore);

            return View("Posts", model);
        }

        public ActionResult Posts(string subName, PostsSortBy? sort, PostsTimeFilter? time, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(subName))
                return Redirect(Url.Subs());

            var model = new SubPostsModel();

            var subs = new List<Guid>();
            Sub sub = null;

            if (subName.Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO: Filter only by subs that want to be including in "all". For now, we will do nothing, which will effectively return all posts.
                model.IsAll = true;
            }
            else
            {
                // the user wants to view a specific sub

                sub = _subDao.GetSubByName(subName);

                if (sub == null)
                    throw new NotFoundException();

                if (_userContext.CurrentUser != null)
                    _subActivityDao.MarkSubActive(_userContext.CurrentUser.Id, sub.Id);

                subs.Add(sub.Id);
            }

            if (sort == null)
                sort = PostsSortBy.Hot; // TODO: get default from sub

            if (time == null)
                time = PostsTimeFilter.All;

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var postIds = _postDao.GetPosts(subs,
                sortby: sort.Value,
                timeFilter: time.Value,
                onlyAll: model.IsAll,
                // anonymous users don't see NSFW content.
                // logged in users only see NSFW if preferences say so.
                // If they want to see NSFW, they will see all content (SFW/NSFW).
                nsfw: _userContext.CurrentUser == null ? false : (_userContext.CurrentUser.ShowNsfw ? (bool?)null : false),
                // we are showing posts for a specific sub, so we can show stickies
                stickyFirst: sub != null,
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            model.Sub = sub != null ? _subWrapper.Wrap(sub.Id, _userContext.CurrentUser) : null;
            model.SortBy = sort.Value;
            model.TimeFilter = time;
            model.Posts = new PagedList<PostWrapped>(_postWrapper.Wrap(postIds, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, postIds.HasMore);

            return View(model);
        }

        public ActionResult Post(string subName, Guid id, CommentSortBy? commentsSort, Guid? commentId = null, int? limit = 100, int context = 0)
        {
            var post = _postDao.GetPostById(id);

            if (post == null)
                throw new HttpException(404, "no post found");

            if (post.Deleted)
            {
                if (_userContext.CurrentUser == null)
                    throw new HttpException(404, "no post found");
                if (post.UserId != _userContext.CurrentUser.Id && !_userContext.CurrentUser.IsAdmin)
                    throw new HttpException(404, "no post found");
            }

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                throw new HttpException(404, "no post found");

            if (!sub.Name.Equals(subName, StringComparison.InvariantCultureIgnoreCase))
                throw new HttpException(404, "no post found"); // TODO: redirect to correct url

            if (!limit.HasValue)
                limit = 100;
            if (limit < 1)
                limit = 100;
            if (limit > 200)
                limit = 200;
            if (context < 0)
                context = 0;

            if (!commentsSort.HasValue)
                commentsSort = CommentSortBy.Best; // TODO: get suggested sort for this link, and if none, from the sub

            var model = new PostDetailsModel();

            model.Post = _postWrapper.Wrap(id, _userContext.CurrentUser);
            model.Sub = _subWrapper.Wrap(sub.Id, _userContext.CurrentUser);
            model.Comments = new CommentListModel();
            model.Comments.SortBy = commentsSort.Value;
            model.ViewingSpecificComment = commentId.HasValue;

            try
            {
                var commentTree = _commentDao.GetCommentTree(model.Post.Post.Id);
                var commentTreeSorter = _commentDao.GetCommentTreeSorter(model.Post.Post.Id, model.Comments.SortBy);
                var commentTreeContext = _commentTreeContextBuilder.Build(commentTree, commentTreeSorter,
                    comment: commentId, limit: limit, maxDepth: 5, context: context);
                commentTreeContext.Sort = model.Comments.SortBy;
                model.Comments.CommentNodes = _commentNodeHierarchyBuilder.Build(commentTree, commentTreeContext,
                    _userContext.CurrentUser);
            }
            catch (CommentNotFoundException ex)
            {
                throw new NotFoundException();
            }

            return View(model);
        }

        [SkimurAuthorize]
        public ActionResult Create(string subName = null, string type = null)
        {
            SubWrapped sub = null;

            if (!string.IsNullOrEmpty(subName))
            {
                sub = _subWrapper.Wrap(_subDao.GetSubByName(subName), _userContext.CurrentUser);
                if (sub == null) throw new HttpException(404, "sub not found");
            }

            if (!string.IsNullOrEmpty(type))
            {
                type = type.ToLower();
            }

            // this prevents the case of the url "{subName}" to override the "SubName" property of the view model.
            ModelState.Clear();

            return View(new CreatePostModel
            {
                PostToSub = sub != null ? sub.Sub.Name : null,
                Sub = sub,
                PostType = type == "text" ? PostType.Text : PostType.Link,
                NotifyReplies = true
            });
        }

        [SkimurAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePostModel model, string subName = null)
        {
            SubWrapped sub = null;

            if (!string.IsNullOrEmpty(subName))
            {
                sub = _subWrapper.Wrap(_subDao.GetSubByName(subName), _userContext.CurrentUser);
                if (sub == null) throw new HttpException(404, "sub not found");
            }

            model.Sub = sub;

            var response = _commandBus.Send<CreatePost, CreatePostResponse>(new CreatePost
            {
                CreatedByUserId = _userContext.CurrentUser.Id,
                IpAddress = Request.UserHostAddress,
                Title = model.Title,
                Url = model.Url,
                Content = model.Content,
                PostType = model.PostType,
                SubName = model.PostToSub,
                NotifyReplies = model.NotifyReplies
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                ModelState.AddModelError(string.Empty, response.Error);
                return View(model);
            }

            if (!response.PostId.HasValue)
            {
                // TODO: log
                ModelState.AddModelError(string.Empty, "Unknown error creating post.");
                return View(model);
            }

            // todo: success message

            return Redirect(Url.Post(model.PostToSub, response.PostId.Value, response.Title));
        }
    }
}
