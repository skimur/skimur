using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Settings;
using Membership.Services;
using Skimur.Web.Models;
using Skimur.Web.Mvc;
using Subs;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class SubsController : BaseController
    {
        private readonly IContextService _contextService;
        private readonly ISubDao _subDao;
        private readonly IMapper _mapper;
        private readonly ICommandBus _commandBus;
        private readonly IUserContext _userContext;
        private readonly IPostDao _postDao;
        private readonly IVoteDao _voteDao;
        private readonly ICommentDao _commentDao;
        private readonly IPermissionDao _permissionDao;
        private readonly ICommentNodeHierarchyBuilder _commentNodeHierarchyBuilder;
        private readonly ICommentTreeContextBuilder _commentTreeContextBuilder;
        private readonly IPostWrapper _postWrapper;
        private readonly ISubWrapper _subWrapper;
        private readonly ICommentWrapper _commentWrapper;
        private readonly IMembershipService _membershipService;
        private readonly ISettingsProvider<SubSettings> _subSettings;
        private readonly ISubActivityDao _subActivityDao;

        public SubsController(IContextService contextService,
            ISubDao subDao,
            IMapper mapper,
            ICommandBus commandBus,
            IUserContext userContext,
            IPostDao postDao,
            IVoteDao voteDao,
            ICommentDao commentDao,
            IPermissionDao permissionDao,
            ICommentNodeHierarchyBuilder commentNodeHierarchyBuilder,
            ICommentTreeContextBuilder commentTreeContextBuilder,
            IPostWrapper postWrapper,
            ISubWrapper subWrapper,
            ICommentWrapper commentWrapper,
            IMembershipService membershipService,
            ISettingsProvider<SubSettings> subSettings,
            ISubActivityDao subActivityDao)
        {
            _contextService = contextService;
            _subDao = subDao;
            _mapper = mapper;
            _commandBus = commandBus;
            _userContext = userContext;
            _postDao = postDao;
            _voteDao = voteDao;
            _commentDao = commentDao;
            _permissionDao = permissionDao;
            _commentNodeHierarchyBuilder = commentNodeHierarchyBuilder;
            _commentTreeContextBuilder = commentTreeContextBuilder;
            _postWrapper = postWrapper;
            _subWrapper = subWrapper;
            _commentWrapper = commentWrapper;
            _membershipService = membershipService;
            _subSettings = subSettings;
            _subActivityDao = subActivityDao;
        }

        public ActionResult Index(string query)
        {
            ViewBag.Query = query;

            return View(_subWrapper.Wrap(
                _subDao.GetAllSubs(query,
                    sortBy: !string.IsNullOrEmpty(query) ? SubsSortBy.Relevance : SubsSortBy.Subscribers,
                    nsfw: _userContext.CurrentNsfw),
                _userContext.CurrentUser));
        }

        public ActionResult Frontpage(PostsSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            var subs = _contextService.GetSubscribedSubIds();

            // if the user is not subscribe to any subs, show the default content.
            if (subs.Count == 0)
                subs = _subDao.GetDefaultSubs();

            if (sort == null)
                sort = PostsSortBy.Hot;

            if (time == null)
                time = TimeFilter.All;

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

        public ActionResult Posts(string name, PostsSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(name))
                return Redirect(Url.Subs());

            var model = new SubPostsModel();

            var subs = new List<Guid>();
            Sub sub = null;

            if (name.Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO: Filter only by subs that want to be including in "all". For now, we will do nothing, which will effectively return all posts.
                model.IsAll = true;
            }
            else
            {
                // the user wants to view a specific sub

                sub = _subDao.GetSubByName(name);

                if (sub == null)
                    return Redirect(Url.Subs(name));

                if (_userContext.CurrentUser != null)
                    _subActivityDao.MarkSubActive(_userContext.CurrentUser.Id, sub.Id);

                subs.Add(sub.Id);
            }

            if (sort == null)
                sort = PostsSortBy.Hot; // TODO: get default from sub

            if (time == null)
                time = TimeFilter.All;

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
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            model.Sub = sub != null ? _subWrapper.Wrap(sub.Id, _userContext.CurrentUser) : null;
            model.SortBy = sort.Value;
            model.TimeFilter = time;
            model.Posts = new PagedList<PostWrapped>(_postWrapper.Wrap(postIds, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, postIds.HasMore);

            return View(model);
        }

        public ActionResult Post(string subName, Guid id, CommentSortBy? commentsSort, Guid? commentId = null, int? limit = 100)
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

            if (!commentsSort.HasValue)
                commentsSort = CommentSortBy.Best; // TODO: get suggested sort for this link, and if none, from the sub

            var model = new PostDetailsModel();

            model.Post = _postWrapper.Wrap(id, _userContext.CurrentUser);
            model.Sub = _subWrapper.Wrap(sub.Id, _userContext.CurrentUser);
            model.Comments = new CommentListModel();
            model.Comments.SortBy = commentsSort.Value;

            var commentTree = _commentDao.GetCommentTree(model.Post.Post.Id);
            var commentTreeSorter = _commentDao.GetCommentTreeSorter(model.Post.Post.Id, model.Comments.SortBy);
            var commentTreeContext = _commentTreeContextBuilder.Build(commentTree, commentTreeSorter, comment: commentId, limit: limit, maxDepth: 5);
            commentTreeContext.Sort = model.Comments.SortBy;
            model.Comments.CommentNodes = _commentNodeHierarchyBuilder.Build(commentTree, commentTreeContext, _userContext.CurrentUser);

            return View(model);
        }

        [Ajax, HttpPost]
        public ActionResult MoreComments(Guid postId, CommentSortBy? sort, string children, int depth)
        {
            if (!sort.HasValue)
                sort = CommentSortBy.Best; // TODO: get suggested sort for this link, and if none, from the sub

            var commentTree = _commentDao.GetCommentTree(postId);
            var commentTreeSorter = _commentDao.GetCommentTreeSorter(postId, sort.Value);
            var commentTreeContext = _commentTreeContextBuilder.Build(commentTree,
                commentTreeSorter,
                children.Split(',').Select(x => Guid.Parse(x)).ToList(),
                limit: 20,
                maxDepth: 5);
            commentTreeContext.Sort = sort.Value;

            var model = new CommentListModel();
            model.SortBy = sort.Value;
            model.CommentNodes = _commentNodeHierarchyBuilder.Build(commentTree, commentTreeContext, _userContext.CurrentUser);

            return Json(new
            {
                success = true,
                error = (string)null,
                html = RenderView("_CommentNodes", model)
            });
        }

        public ActionResult SearchSub(string name, string query, PostsSearchSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(name))
                return Redirect(Url.Subs());

            if (sort == null)
                sort = PostsSearchSortBy.Relevance;

            if (time == null)
                time = TimeFilter.All;

            var sub = _subDao.GetSubByName(name);

            if (sub == null)
                return Redirect(Url.Subs(name));

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var model = new SearchResultsModel();
            model.Query = query;
            model.SortBy = sort.Value;
            model.TimeFilter = time.Value;
            model.ResultType = SearchResultType.Post;
            model.LimitingToSub = _subWrapper.Wrap(sub.Id, _userContext.CurrentUser);

            var postIds = _postDao.QueryPosts(query,
               model.LimitingToSub.Sub.Id,
               sortBy: sort.Value,
               timeFilter: time.Value,
               nsfw: _userContext.CurrentNsfw,
               skip: ((pageNumber - 1) * pageSize),
               take: pageSize);

            if (!string.IsNullOrEmpty(model.Query))
                model.Posts = new PagedList<PostWrapped>(
                    _postWrapper.Wrap(postIds, _userContext.CurrentUser),
                    pageNumber.Value,
                    pageSize.Value,
                    postIds.HasMore);

            return View("Search", model);
        }

        public ActionResult SearchSite(string query,
            PostsSearchSortBy? sort,
            TimeFilter? time,
            SearchResultType? resultType,
            int? pageNumber,
            int? pageSize)
        {
            if (sort == null)
                sort = PostsSearchSortBy.Relevance;

            if (time == null)
                time = TimeFilter.All;

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var model = new SearchResultsModel();
            model.Query = query;
            model.SortBy = sort.Value;
            model.TimeFilter = time.Value;
            model.ResultType = resultType;

            if (!string.IsNullOrEmpty(model.Query))
            {
                SeekedList<Guid> postIds = null;
                SeekedList<Guid> subIds = null;

                switch (resultType)
                {
                    case null:
                        postIds = _postDao.QueryPosts(query,
                            model.LimitingToSub != null ? model.LimitingToSub.Sub.Id : (Guid?)null,
                            sortBy: sort.Value,
                            timeFilter: time.Value,
                            skip: ((pageNumber - 1) * pageSize),
                            take: pageSize);
                        subIds = _subDao.GetAllSubs(model.Query,
                            sortBy: SubsSortBy.Relevance,
                            nsfw: _userContext.CurrentNsfw,
                            skip: ((pageNumber - 1) * pageSize),
                            take: pageSize);
                        break;
                    case SearchResultType.Post:
                        postIds = _postDao.QueryPosts(query,
                            model.LimitingToSub != null ? model.LimitingToSub.Sub.Id : (Guid?)null,
                            sortBy: sort.Value,
                            timeFilter: time.Value,
                            skip: ((pageNumber - 1) * pageSize),
                            take: pageSize);
                        break;
                    case SearchResultType.Sub:
                        subIds = _subDao.GetAllSubs(model.Query,
                              sortBy: SubsSortBy.Relevance,
                              nsfw: _userContext.CurrentNsfw,
                              skip: ((pageNumber - 1) * pageSize),
                              take: pageSize);
                        break;
                    default:
                        throw new Exception("unknown result type");
                }

                if (postIds != null)
                    model.Posts = new PagedList<PostWrapped>(_postWrapper.Wrap(postIds, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, postIds.HasMore);

                if (subIds != null)
                    model.Subs = new PagedList<SubWrapped>(_subWrapper.Wrap(subIds, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, subIds.HasMore);
            }

            return View("Search", model);
        }

        public ActionResult Random()
        {
            var randomSubId = _subDao.GetRandomSub(nsfw: _userContext.CurrentNsfw);

            if (randomSubId == null)
                return Redirect(Url.Subs());

            var randomSub = _subDao.GetSubById(randomSubId.Value);

            if (randomSub == null)
                return Redirect(Url.Subs());

            return Redirect(Url.Sub(randomSub.Name));
        }

        public ActionResult Edit(string id)
        {
            var name = id;

            if (string.IsNullOrEmpty(name))
                return Redirect(Url.Subs());

            var sub = _subDao.GetSubByName(name);

            if (sub == null)
                return Redirect(Url.Subs(name));

            if (!_permissionDao.CanUserManageSubConfig(_userContext.CurrentUser, sub.Id))
                throw new UnauthorizedException();

            var model = _mapper.Map<Sub, CreateEditSubModel>(sub);
            model.IsEditing = true;

            if (!_userContext.CurrentUser.IsAdmin)
                model.IsDefault = null; // the user can't edit this

            return View(model);
        }

        [SkimurAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CreateEditSubModel model)
        {
            var name = id;
            model.IsEditing = true;
            model.Name = name;

            if (string.IsNullOrEmpty(name))
                return Redirect(Url.Subs());

            var sub = _subDao.GetSubByName(name);

            if (sub == null)
                return Redirect(Url.Subs(name));

            if (!_permissionDao.CanUserManageSubConfig(_userContext.CurrentUser, sub.Id))
                throw new UnauthorizedException();

            var response = _commandBus.Send<EditSub, EditSubResponse>(new EditSub
            {
                EditedByUserId = _userContext.CurrentUser.Id,
                Name = name,
                Description = model.Description,
                SidebarText = model.SidebarText,
                SubmissionText = model.SubmissionText,
                Type = model.SubType,
                IsDefault = model.IsDefault,
                Nsfw = model.Nsfw,
                InAll = model.InAll
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                ModelState.AddModelError(string.Empty, response.Error);
                return View(model);
            }

            // todo: success message

            return View(model);
        }

        [SkimurAuthorize]
        public ActionResult Create()
        {
            var model = new CreateEditSubModel();

            // admins can create default subs!
            // not null means editable.
            if (_userContext.CurrentUser.IsAdmin)
                model.IsDefault = false;

            model.InAll = true;

            return View(model);
        }

        [SkimurAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditSubModel model)
        {
            var response = _commandBus.Send<CreateSub, CreateSubResponse>(new CreateSub
            {
                CreatedByUserId = _userContext.CurrentUser.Id,
                Name = model.Name,
                Description = model.Description,
                SidebarText = model.SidebarText,
                SubmissionText = model.SubmissionText,
                Type = model.SubType,
                IsDefault = model.IsDefault,
                InAll = model.InAll,
                Nsfw = model.Nsfw
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                ModelState.AddModelError(string.Empty, response.Error);
                return View(model);
            }

            // todo: success message

            return Redirect(Url.EditSub(response.SubName));
        }

        [SkimurAuthorize]
        public ActionResult CreatePost(string subName = null, string type = null)
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
                SubName = sub != null ? sub.Sub.Name : null,
                Sub = sub,
                PostType = type == "text" ? PostType.Text : PostType.Link,
                NotifyReplies = true
            });
        }

        [SkimurAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(CreatePostModel model, string subName = null)
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
                SubName = model.SubName,
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

            return Redirect(Url.Post(model.SubName, response.PostId.Value, response.Title));
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult CreateComment(CreateCommentModel model)
        {
            var dateCreated = Common.CurrentTime();
            var response = _commandBus.Send<CreateComment, CreateCommentResponse>(new CreateComment
            {
                PostId = model.PostId,
                ParentId = model.ParentId,
                DateCreated = dateCreated,
                AuthorIpAddress = Request.UserHostAddress,
                AuthorUserName = _userContext.CurrentUser.UserName,
                Body = model.Body,
                SendReplies = model.SendReplies
            });

            if (!string.IsNullOrEmpty(response.Error))
                return CommonJsonResult(response.Error);

            if (!response.CommentId.HasValue)
                throw new Exception("No error was given, which indicates success, but no comment id was returned.");

            var node = _commentWrapper.Wrap(response.CommentId.Value, _userContext.CurrentUser);

            return Json(new
            {
                success = true,
                commentId = response.CommentId,
                html = RenderView("_CommentNode", new CommentNode(node))
            });
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult EditComment(EditCommentModel model)
        {
            var response = _commandBus.Send<EditComment, EditCommentResponse>(new EditComment
            {
                DateEdited = Common.CurrentTime(),
                CommentId = model.CommentId,
                Body = model.Body
            });

            if (!string.IsNullOrEmpty(response.Error))
                return CommonJsonResult(response.Error);

            var html = RenderView("_CommentBody", new CommentNode(_commentWrapper.Wrap(model.CommentId, _userContext.CurrentUser)));

            return Json(new
            {
                success = true,
                commentId = model.CommentId,
                // we don't render the whole comment, just the body.
                // this is because we want to leave the children in-tact on the ui
                html
            });
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult DeleteComment(Guid commentId)
        {
            var response = _commandBus.Send<DeleteComment, DeleteCommentResponse>(new DeleteComment
            {
                CommentId = commentId,
                UserName = _userContext.CurrentUser.UserName,
                DateDeleted = Common.CurrentTime()
            });

            return CommonJsonResult(response.Error);
        }

        public ActionResult TopBar()
        {
            return PartialView("_TopBar", _subWrapper.Wrap(_contextService.GetSubscribedSubIds(), _userContext.CurrentUser));
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult Subscribe(string subName)
        {
            var response = _commandBus.Send<SubcribeToSub, SubcribeToSubResponse>(new SubcribeToSub
            {
                UserName = _userContext.CurrentUser.UserName,
                SubName = subName
            });

            return CommonJsonResult(response.Success, response.Error);
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult UnSubscribe(string subName)
        {
            var response = _commandBus.Send<UnSubcribeToSub, UnSubcribeToSubResponse>(new UnSubcribeToSub
            {
                UserId = _userContext.CurrentUser.Id,
                SubName = subName
            });

            return CommonJsonResult(response.Success, response.Error);
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult VotePost(Guid postId, VoteType type)
        {
            _commandBus.Send(new CastVoteForPost
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
                DateCasted = Common.CurrentTime(),
                IpAddress = Request.UserHostAddress,
                VoteType = type
            });

            return CommonJsonResult(true);
        }

        [SkimurAuthorize, Ajax]
        public ActionResult UnVotePost(Guid postId)
        {
            _commandBus.Send(new CastVoteForPost
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
                DateCasted = Common.CurrentTime(),
                IpAddress = Request.UserHostAddress,
                VoteType = null /*no vote*/
            });

            return CommonJsonResult(true);
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult VoteComment(Guid commentId, VoteType type)
        {
            _commandBus.Send(new CastVoteForComment
            {
                UserId = _userContext.CurrentUser.Id,
                CommentId = commentId,
                DateCasted = Common.CurrentTime(),
                IpAddress = Request.UserHostAddress,
                VoteType = type
            });

            return CommonJsonResult(true);
        }

        [SkimurAuthorize, Ajax, HttpPost]
        public ActionResult UnVoteComment(Guid commentId)
        {
            _commandBus.Send(new CastVoteForComment
            {
                UserId = _userContext.CurrentUser.Id,
                CommentId = commentId,
                DateCasted = Common.CurrentTime(),
                IpAddress = Request.UserHostAddress,
                VoteType = null /*no vote*/
            });

            return CommonJsonResult(true);
        }

        [Ajax]
        public ActionResult SubmissionText(string subName, Guid? subId)
        {
            var sub = subId.HasValue ? _subDao.GetSubById(subId.Value) : _subDao.GetSubByName(subName);

            if (sub == null)
                return Json(new
                {
                    submission_text = "",
                    sub_found = false,
                    success = true
                }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                submission_text = sub.SubmissionTextFormatted,
                sub_found = true,
                name = sub.Name,
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}