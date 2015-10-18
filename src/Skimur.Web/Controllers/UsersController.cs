using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Membership.Services;
using Skimur.Web.Models;
using Subs.ReadModel;
using Subs.Services;

namespace Skimur.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IModerationDao _moderationDao;
        private readonly IKarmaDao _karmaDao;
        private readonly ICommentDao _commentDao;
        private readonly ICommentWrapper _commentWrapper;
        private readonly IPostDao _postDao;
        private readonly IPostWrapper _postWrapper;
        private readonly IUserContext _userContext;
        private readonly IContextService _contextService;

        public UsersController(IMembershipService membershipService,
            ISubDao subDao,
            IModerationDao moderationDao,
            IKarmaDao karmaDao,
            ICommentDao commentDao,
            ICommentWrapper commentWrapper,
            IPostDao postDao,
            IPostWrapper postWrapper,
            IUserContext userContext,
            IContextService contextService)
        {
            _membershipService = membershipService;
            _subDao = subDao;
            _moderationDao = moderationDao;
            _karmaDao = karmaDao;
            _commentDao = commentDao;
            _commentWrapper = commentWrapper;
            _postDao = postDao;
            _postWrapper = postWrapper;
            _userContext = userContext;
            _contextService = contextService;
        }

        public ActionResult User(string userName)
        {
            ViewBag.NavigationKey = "overview";

            return View(BuildUserModel(userName));
        }

        public ActionResult Comments(string userName, UserViewModel.SortByEnum? sort = null, UserViewModel.TimeFilterEnum? filter = null, int? pageNumber = null, int? pageSize = null)
        {
            ViewBag.NavigationKey = "comments";

            var model = BuildUserModel(userName);

            if (sort == null)
                sort = UserViewModel.SortByEnum.New;

            if (filter == null)
                filter = UserViewModel.TimeFilterEnum.All;

            if (sort.Value != UserViewModel.SortByEnum.Top && sort.Value != UserViewModel.SortByEnum.Controversial)
                filter = UserViewModel.TimeFilterEnum.All;

            model.SortBy = sort.Value;
            model.TimeFilter = filter.Value;

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var comments = _commentDao.GetCommentsForUser(model.User.Id,
                sortBy: (CommentSortBy)Enum.Parse(typeof(CommentSortBy), sort.Value.ToString()),
                timeFilter: (CommentsTimeFilter)Enum.Parse(typeof(CommentsTimeFilter), filter.Value.ToString()),
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            model.Comments = new PagedList<CommentWrapped>(
                _commentWrapper.Wrap(comments, _userContext.CurrentUser),
                pageNumber.Value,
                pageSize.Value,
                comments.HasMore);

            return View(model);
        }

        public ActionResult Posts(string userName, UserViewModel.SortByEnum? sort = null, UserViewModel.TimeFilterEnum? filter = null, int? pageNumber = null, int? pageSize = null)
        {
            ViewBag.NavigationKey = "posts";

            var model = BuildUserModel(userName);

            if (sort == null)
                sort = UserViewModel.SortByEnum.New;

            if (filter == null)
                filter = UserViewModel.TimeFilterEnum.All;

            if (sort.Value != UserViewModel.SortByEnum.Top && sort.Value != UserViewModel.SortByEnum.Controversial)
                filter = UserViewModel.TimeFilterEnum.All;

            model.SortBy = sort.Value;
            model.TimeFilter = filter.Value;
            
            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;
            
            var posts = _postDao.GetPosts(
                userId: model.User.Id,
                sortby: (PostsSortBy)Enum.Parse(typeof(PostsSortBy), sort.Value.ToString()),
                timeFilter: (PostsTimeFilter)Enum.Parse(typeof(PostsTimeFilter), filter.Value.ToString()),
                nsfw: _userContext.CurrentNsfw,
                skip: ((pageNumber - 1) * pageSize),
                take: pageSize);

            model.Posts = new PagedList<PostWrapped>(
               _postWrapper.Wrap(posts, _userContext.CurrentUser),
               pageNumber.Value,
               pageSize.Value,
               posts.HasMore);

            return View(model);
        }

        private UserViewModel BuildUserModel(string userName)
        {
            var user = _membershipService.GetUserByUserName(userName);

            if (user == null)
                throw new NotFoundException();

            var model = new UserViewModel();
            model.User = user;

            var moderatedSubs = _moderationDao.GetSubsModeratoredByUser(user.Id);

            if (moderatedSubs.Count > 0)
            {
                model.IsModerator = true;
                model.ModeratingSubs = _subDao.GetSubsByIds(moderatedSubs).Select(x => x.Name).ToList();
            }

            var kudos = _karmaDao.GetKarma(user.Id);

            model.CommentKudos = kudos.Keys.Where(x => x.Type == KarmaType.Comment).Sum(x => kudos[x]);
            model.PostKudos = kudos.Keys.Where(x => x.Type == KarmaType.Post).Sum(x => kudos[x]);

            return model;
        }
    }
}
