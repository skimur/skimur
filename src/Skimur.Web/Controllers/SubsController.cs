using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Utils;
using Skimur.Web.Models;
using Subs;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class SubsController : Controller
    {
        private readonly IContextService _contextService;
        private readonly ISubDao _subDao;
        private readonly IMapper _mapper;
        private readonly ICommandBus _commandBus;
        private readonly IUserContext _userContext;
        private readonly IPostDao _postDao;
        private readonly IVoteDao _voteDao;
        private readonly ICommentDao _commentDao;

        public SubsController(IContextService contextService,
            ISubDao subDao,
            IMapper mapper,
            ICommandBus commandBus,
            IUserContext userContext,
            IPostDao postDao,
            IVoteDao voteDao,
            ICommentDao commentDao)
        {
            _contextService = contextService;
            _subDao = subDao;
            _mapper = mapper;
            _commandBus = commandBus;
            _userContext = userContext;
            _postDao = postDao;
            _voteDao = voteDao;
            _commentDao = commentDao;
        }

        public ActionResult Index(string query)
        {
            var subscribedSubs = _contextService.GetSubscribedSubNames();
            var allSubs = _subDao.GetAllSubs(query, !string.IsNullOrEmpty(query) ? SubsSortBy.Relevance : SubsSortBy.Subscribers).Select(x =>
            {
                var model = _mapper.Map<Sub, SubModel>(x);

                if (subscribedSubs.Contains(model.Name))
                    model.IsSubscribed = true;

                return model;
            }).ToList();

            ViewBag.Query = query;

            return View(allSubs);
        }

        public ActionResult Frontpage(PostsSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            var subs = _contextService.GetSubscribedSubNames();

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

            var model = new SubPostsModel();
            model.SortBy = sort.Value;
            model.TimeFilter = time;
            if (subs.Any()) // maybe the user hasn't subscribed to any subs?
                model.Posts = PagedList<PostModel>.Build(_postDao.GetPosts(subs, sort.Value, time.Value, ((pageNumber - 1) * pageSize), pageSize), MapPost, pageNumber.Value, pageSize.Value);

            return View("Posts", model);
        }

        public ActionResult Posts(string name, PostsSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(name))
                return Redirect(Url.Subs());

            var subs = new List<string>();

            Sub sub = null;

            if (name.Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO: Filter only by subs that want to be including in "all". For now, we will do nothing, which will effectively return all posts.
            }
            else
            {
                // the user wants to view a specific sub

                sub = _subDao.GetSubByName(name);

                if (sub == null)
                    return Redirect(Url.Subs(name));

                subs.Add(sub.Name);
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

            var model = new SubPostsModel();
            model.Sub = MapSub(sub);
            model.SortBy = sort.Value;
            model.TimeFilter = time;
            model.Posts = PagedList<PostModel>.Build(
                _postDao.GetPosts(subs, sort.Value, time.Value, ((pageNumber - 1) * pageSize), pageSize),
                MapPost,
                pageNumber.Value,
                pageSize.Value);

            return View(model);
        }

        public ActionResult Post(string subName, string slug)
        {
            var post = _postDao.GetPostBySlug(slug);

            if (post == null)
                throw new HttpException(404, "no post found");

            if(!post.SubName.Equals(subName, StringComparison.InvariantCultureIgnoreCase))
                throw new HttpException(404, "no post found");

            var sub = _subDao.GetSubByName(subName);

            if(sub == null)
                throw new HttpException(404, "no post found");

            var model = new PostDetailsModel();
            model.Post = MapPost(post);
            model.Sub = MapSub(sub);
            model.Comments = new CommentListModel();

            for (var x = 0; x < 10; x++)
            {
                var time = Common.CurrentTime().Subtract(TimeSpan.FromDays(5));
                var comment = new CommentModel();
                comment.Id = GuidUtil.NewSequentialId();
                comment.AuthorUserName = "testuser";
                comment.BodyFormatted = "<p>test " + x + "</p>";
                comment.Body = "test " + x;
                comment.DateCreated = time;

                for (var y = 0; y <= 1; y++)
                {
                    var child = new CommentModel();
                    child.Id = GuidUtil.NewSequentialId();
                    child.ParentId = comment.Id;
                    child.AuthorUserName = "testuser";
                    child.BodyFormatted = "<p>child " + y + "</p>";
                    child.Body = "child " + y;
                    child.DateCreated = time;

                    if(comment.Children == null)
                        comment.Children = new List<CommentModel>();

                    comment.Children.Add(child);

                    for (var z = 0; z <= 2; z++)
                    {
                        var grandChild = new CommentModel();
                        grandChild.Id = GuidUtil.NewSequentialId();
                        grandChild.ParentId = child.Id;
                        grandChild.AuthorUserName = "testuser";
                        grandChild.BodyFormatted = "<p>grand child " + z + "</p>";
                        grandChild.Body = "grand child " + z;
                        grandChild.DateCreated = time;

                        if (child.Children == null)
                            child.Children = new List<CommentModel>();

                        child.Children.Add(grandChild);
                    }
                }

                model.Comments.Comments.Add(comment);
            }

            return View(model);
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
            model.LimitingToSub = MapSub(sub);

            if (!string.IsNullOrEmpty(model.Query))
                model.Posts = PagedList<PostModel>.Build(
                    _postDao.QueryPosts(query,
                        model.LimitingToSub.Name,
                        sort.Value,
                        time.Value,
                        ((pageNumber - 1) * pageSize),
                        pageSize),
                    MapPost,
                    pageNumber.Value,
                    pageSize.Value);

            return View("Search", model);
        }

        public ActionResult SearchSite(string query, PostsSearchSortBy? sort, TimeFilter? time, SearchResultType? resultType, int? pageNumber, int? pageSize)
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
                switch (resultType)
                {
                    case null:
                        model.Posts = PagedList<PostModel>.Build(
                            _postDao.QueryPosts(query, model.LimitingToSub != null ? model.LimitingToSub.Name : null, sort.Value, time.Value, ((pageNumber - 1) * pageSize), pageSize),
                            MapPost,
                            pageNumber.Value,
                            pageSize.Value);
                        model.Subs = PagedList<SubModel>.Build(
                            _subDao.GetAllSubs(model.Query, SubsSortBy.Relevance, ((pageNumber - 1) * pageSize), pageSize),
                            MapSub,
                            pageNumber.Value,
                            pageSize.Value);
                        break;
                    case SearchResultType.Post:
                        model.Posts = PagedList<PostModel>.Build(
                            _postDao.QueryPosts(query, model.LimitingToSub != null ? model.LimitingToSub.Name : null, sort.Value, time.Value, ((pageNumber - 1) * pageSize), pageSize),
                            MapPost,
                            pageNumber.Value,
                            pageSize.Value);
                        break;
                    case SearchResultType.Sub:
                        model.Subs = PagedList<SubModel>.Build(
                            _subDao.GetAllSubs(model.Query, SubsSortBy.Relevance, ((pageNumber - 1) * pageSize), pageSize),
                            MapSub,
                            pageNumber.Value,
                            pageSize.Value);
                        break;
                    default:
                        throw new Exception("unknown result type");
                }

            }

            return View("Search", model);
        }

        public ActionResult Random()
        {
            var randomSub = _subDao.GetRandomSub();

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

            var model = _mapper.Map<Sub, CreateEditSubModel>(sub);
            model.IsEditing = true;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CreateEditSubModel model)
        {
            var name = id;
            model.IsEditing = true;
            model.Name = name;

            var response = _commandBus.Send<EditSub, EditSubResponse>(new EditSub
            {
                EditedByUserId = _userContext.CurrentUser.Id,
                Name = name,
                Description = model.Description,
                SidebarText = model.SidebarText
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                ModelState.AddModelError(string.Empty, response.Error);
                return View(model);
            }

            // todo: success message


            return View(model);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View(new CreateEditSubModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditSubModel model)
        {
            var response = _commandBus.Send<CreateSub, CreateSubResponse>(new CreateSub
            {
                CreatedByUserId = _userContext.CurrentUser.Id,
                Name = model.Name,
                Description = model.Description,
                SidebarText = model.SidebarText
            });

            if (!string.IsNullOrEmpty(response.Error))
            {
                ModelState.AddModelError(string.Empty, response.Error);
                return View(model);
            }

            // todo: success message

            return Redirect(Url.EditSub(response.SubName));
        }

        [Authorize]
        public ActionResult CreatePost()
        {
            return View(new CreatePostModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(CreatePostModel model)
        {
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

            // todo: success message

            return Redirect(Url.Post(model.SubName, response.Slug, response.Title));
        }

        [HttpPost]
        public ActionResult CreateComment(CreateCommentModel model)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to comment."
                });
            }

            try
            {
                var response = _commandBus.Send<CreateComment, CreateCommentResponse>(new CreateComment
                {
                    PostSlug = model.PostSlug,
                    ParentId = model.ParentId,
                    AuthorIpAddress = Request.UserHostAddress,
                    AuthorUserName = _userContext.CurrentUser.UserName,
                    Body = model.Body,
                    SendReplies = model.SendReplies
                });

                if (!string.IsNullOrEmpty(response.Error))
                {
                    return Json(new
                    {
                        success = false,
                        error = response.Error
                    });
                }

                return Json(new
                {
                    success = true,
                    commentId = response.CommentId,
                    body = response.Body,
                    formattedBody = response.FormattedBody
                });
            }
            catch (Exception ex)
            {
                // TODO: log
                return Json(new
                {
                    success = false,
                    error = "An unexpected error occured."
                });
            }
        }

        public ActionResult SideBar()
        {
            var allSubs = _subDao.GetSubByNames(_contextService.GetSubscribedSubNames()).Select(x =>
            {
                var model = _mapper.Map<Sub, SubModel>(x);
                model.IsSubscribed = true;
                return model;
            }).ToList();

            return PartialView("_SideBar", allSubs);
        }

        public ActionResult TopBar()
        {
            var allSubs = _subDao.GetSubByNames(_contextService.GetSubscribedSubNames()).Select(x =>
            {
                var model = _mapper.Map<Sub, SubModel>(x);
                model.IsSubscribed = true;
                return model;
            }).ToList();

            return PartialView("_TopBar", allSubs);
        }

        [HttpPost]
        public JsonResult Subscribe(string subName)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in subscribe to a sub."
                });
            }

            var response = _commandBus.Send<SubcribeToSub, SubcribeToSubResponse>(new SubcribeToSub
            {
                UserName = _userContext.CurrentUser.UserName,
                SubName = subName
            });

            return Json(new
            {
                success = response.Success,
                error = response.Error
            });
        }

        [HttpPost]
        public JsonResult UnSubscribe(string subName)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in subscribe to a sub."
                });
            }

            var response = _commandBus.Send<UnSubcribeToSub, UnSubcribeToSubResponse>(new UnSubcribeToSub
            {
                UserName = _userContext.CurrentUser.UserName,
                SubName = subName
            });

            return Json(new
            {
                success = response.Success,
                error = response.Error
            });
        }

        [HttpPost]
        public ActionResult Vote(string postSlug, VoteType type)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to vote for an item."
                });
            }

            _commandBus.Send(new CaseVote
             {
                 UserName = _userContext.CurrentUser.UserName,
                 PostSlug = postSlug,
                 DateCasted = Common.CurrentTime(),
                 IpAddress = Request.UserHostAddress,
                 VoteType = type
             });

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }

        public ActionResult UnVote(string postSlug)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to unvote for an item."
                });
            }

            _commandBus.Send(new CaseVote
            {
                UserName = _userContext.CurrentUser.UserName,
                PostSlug = postSlug,
                DateCasted = Common.CurrentTime(),
                IpAddress = Request.UserHostAddress,
                VoteType = null /*no vote*/
            });

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }

        public ActionResult ModerationSideBar(string subName)
        {
            var sub = _subDao.GetSubByName(subName);

            if (sub == null)
                return new EmptyResult();

            var model = new ModerationSideBarModel();
            model.SubName = sub.Name;

            if (_userContext.CurrentUser != null)
            {
                model.IsModerator = _subDao.CanUserModerateSub(_userContext.CurrentUser.UserName, sub.Name);
            }
            else
            {
                // we only show list of mods if the requesting user is not a mod of this sub
                model.Moderators.AddRange(_subDao.GetAllModsForSub(sub.Name));
            }

            return PartialView("_ModerationSideBar", model);
        }

        private PostModel MapPost(Post post)
        {
            if (post == null)
                return null;
            var result = _mapper.Map<Post, PostModel>(post);
            if (_userContext.CurrentUser != null)
                result.CurrentVote = _voteDao.GetVoteForUserOnPost(_userContext.CurrentUser.UserName, post.Slug);
            return result;
        }

        private SubModel MapSub(Sub sub)
        {
            if (sub == null) return null;
            var result = _mapper.Map<Sub, SubModel>(sub);
            result.IsSubscribed = _contextService.IsSubcribedToSub(sub.Name);
            return result;
        }

        private CommentModel MapComment(Comment comment)
        {
            if (comment == null) return null;
            var result = _mapper.Map<Comment, CommentModel>(comment);
            return result;
        }
    }
}