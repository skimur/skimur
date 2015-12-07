using Membership.Services;
using Microsoft.AspNet.Mvc;
using Skimur.Messaging;
using Skimur.Settings;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs;
using Subs.Commands;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IModerationDao _moderationDao;

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
            ISubActivityDao subActivityDao,
            IModerationDao moderationDao)
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
            _moderationDao = moderationDao;
        }

        public ActionResult Popular(string query, int? pageNumber, int? pageSize)
        {
            ViewBag.NavigationKey = "popular";

            ViewBag.Query = query;

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 24;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var subs = _subDao.GetAllSubs(query,
                sortBy: SubsSortBy.Subscribers,
                nsfw: _userContext.CurrentNsfw,
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            return View("List", new PagedList<SubWrapped>(_subWrapper.Wrap(subs, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, subs.HasMore));
        }

        public ActionResult New(int? pageNumber, int? pageSize)
        {
            ViewBag.NavigationKey = "new";

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 24;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var subs = _subDao.GetAllSubs(sortBy: SubsSortBy.New,
                nsfw: _userContext.CurrentNsfw,
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            return View("List", new PagedList<SubWrapped>(_subWrapper.Wrap(subs, _userContext.CurrentUser), pageNumber.Value, pageSize.Value, subs.HasMore));
        }

        [SkimurAuthorize]
        public ActionResult Subscribed()
        {
            ViewBag.NavigationKey = "subscribed";

            return View("List", new PagedList<SubWrapped>(_subWrapper.Wrap(_contextService.GetSubscribedSubIds(), _userContext.CurrentUser), 1, int.MaxValue, false));
        }

        [SkimurAuthorize]
        public ActionResult Moderating()
        {
            ViewBag.NavigationKey = "moderating";

            return View("List", new PagedList<SubWrapped>(_subWrapper.Wrap(_moderationDao.GetSubsModeratoredByUser(_userContext.CurrentUser.Id), _userContext.CurrentUser), 1, int.MaxValue, false));
        }

        public ActionResult SearchSub(string subName, string query, PostsSearchSortBy? sort, PostsTimeFilter? time, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(subName))
                return Redirect(Url.Subs());

            if (sort == null)
                sort = PostsSearchSortBy.Relevance;

            if (time == null)
                time = PostsTimeFilter.All;

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                return Redirect(Url.Subs(subName));

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
            PostsTimeFilter? time,
            SearchResultType? resultType,
            int? pageNumber,
            int? pageSize)
        {
            if (sort == null)
                sort = PostsSearchSortBy.Relevance;

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

        public ActionResult Edit(string subName)
        {
            if (string.IsNullOrEmpty(subName))
                return Redirect(Url.Subs());

            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                return Redirect(Url.Subs(subName));

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

            AddSuccessMessage("You sub has been succesfully created.");

            return Redirect(Url.Sub(response.SubName));
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
                IpAddress = HttpContext.RemoteAddress(),
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
                IpAddress = HttpContext.RemoteAddress(),
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
                IpAddress = HttpContext.RemoteAddress(),
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
                IpAddress = HttpContext.RemoteAddress(),
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
                });

            return Json(new
            {
                submission_text = sub.SubmissionTextFormatted,
                sub_found = true,
                name = sub.Name,
                success = true
            });
        }
    }
}
