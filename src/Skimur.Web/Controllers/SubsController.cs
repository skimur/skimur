using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Utils;
using Skimur.Web.Models;
using Subs;
using Subs.Commands;
using Subs.ReadModel;
using Subs.Services;

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
            ICommentWrapper commentWrapper)
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
        }

        public ActionResult Index(string query)
        {
            ViewBag.Query = query;
            
            return View(_subWrapper.Wrap(_subDao.GetAllSubs(query, !string.IsNullOrEmpty(query) ? SubsSortBy.Relevance : SubsSortBy.Subscribers), _userContext.CurrentUser));
        }

        public ActionResult Frontpage(PostsSortBy? sort, TimeFilter? time, int? pageNumber, int? pageSize)
        {
            var subs = _contextService.GetSubscribedSubIds();

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

            var postIds = _postDao.GetPosts(subs, sort.Value, time.Value, ((pageNumber - 1)*pageSize), pageSize);
            
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

            var subs = new List<Guid>();

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

            var postIds = _postDao.GetPosts(subs, sort.Value, time.Value, ((pageNumber - 1) * pageSize), pageSize);

            var model = new SubPostsModel();
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

            if(!commentsSort.HasValue)
                commentsSort = CommentSortBy.Best; // TODO: get suggested sort for this link, and if none, from the sub

            var model = new PostDetailsModel();

            model.Post = _postWrapper.Wrap(id, _userContext.CurrentUser);
            model.Sub = _subWrapper.Wrap(sub.Id, _userContext.CurrentUser);
            model.Comments = new CommentListModel();
            model.Comments.SortBy = commentsSort.Value;

            var commentTree = _commentDao.GetCommentTree(model.Post.Post.Id);
            var commentTreeSorter = _commentDao.GetCommentTreeSorter(model.Post.Post.Id, model.Comments.SortBy);
            var commentTreeContext = _commentTreeContextBuilder.Build(commentTree, commentTreeSorter, comment:commentId, limit: limit, maxDepth:5);
            commentTreeContext.Sort = model.Comments.SortBy;
            model.Comments.CommentNodes = _commentNodeHierarchyBuilder.Build(commentTree, commentTreeContext, _userContext.CurrentUser);

            return View(model);
        }

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
               sort.Value,
               time.Value,
               ((pageNumber - 1) * pageSize),
               pageSize);

            if (!string.IsNullOrEmpty(model.Query))
                model.Posts = new PagedList<PostWrapped>(
                    _postWrapper.Wrap(postIds, _userContext.CurrentUser),
                    pageNumber.Value,
                    pageSize.Value,
                    postIds.HasMore);

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
                SeekedList<Guid> postIds = null;
                SeekedList<Guid> subIds = null;

                switch (resultType)
                {
                    case null:
                        postIds = _postDao.QueryPosts(query,
                            model.LimitingToSub != null ? model.LimitingToSub.Sub.Id : (Guid?) null, sort.Value,
                            time.Value, ((pageNumber - 1)*pageSize), pageSize);
                        subIds = _subDao.GetAllSubs(model.Query, SubsSortBy.Relevance, ((pageNumber - 1)*pageSize), pageSize);
                        break;
                    case SearchResultType.Post:
                        postIds = _postDao.QueryPosts(query,
                            model.LimitingToSub != null ? model.LimitingToSub.Sub.Id : (Guid?) null, sort.Value, 
                            time.Value, ((pageNumber - 1)*pageSize), pageSize);
                        break;
                    case SearchResultType.Sub:
                        subIds = _subDao.GetAllSubs(model.Query, SubsSortBy.Relevance, ((pageNumber - 1)*pageSize), pageSize);
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
            var randomSubId = _subDao.GetRandomSub();

            if (randomSubId == null)
                return Redirect(Url.Subs());

            var randomSub = _subDao.GetSubById(randomSubId.Value);

            if(randomSub == null)
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

            if (!response.PostId.HasValue)
            {
                // TODO: log
                ModelState.AddModelError(string.Empty, "Unknown error creating post.");
                return View(model);
            }

            // todo: success message

            return Redirect(Url.Post(model.SubName, response.PostId.Value, response.Title));
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
                {
                    return Json(new
                    {
                        success = false,
                        error = response.Error
                    });
                }

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

        [HttpPost]
        public ActionResult EditComment(EditCommentModel model)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to edit a comment."
                });
            }

            try
            {

                var response = _commandBus.Send<EditComment, EditCommentResponse>(new EditComment
                {
                    DateEdited = Common.CurrentTime(),
                    CommentId = model.CommentId,
                    Body = model.Body
                });

                if (!string.IsNullOrEmpty(response.Error))
                {
                    return Json(new
                    {
                        success = false,
                        error = response.Error
                    });
                }
                
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

        [HttpPost]
        public ActionResult DeleteComment(Guid commentId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to delete a comment."
                });
            }

            try
            {
                var response = _commandBus.Send<DeleteComment, DeleteCommentResponse>(new DeleteComment
                {
                    CommentId = commentId,
                    UserName = _userContext.CurrentUser.UserName,
                    DateDeleted = Common.CurrentTime()
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
                    error = (string)null
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
            return PartialView("_SideBar", _subWrapper.Wrap(_contextService.GetSubscribedSubIds(), _userContext.CurrentUser));
        }

        public ActionResult TopBar()
        {
            return PartialView("_TopBar", _subWrapper.Wrap(_contextService.GetSubscribedSubIds(), _userContext.CurrentUser));
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
                UserId = _userContext.CurrentUser.Id,
                SubName = subName
            });

            return Json(new
            {
                success = response.Success,
                error = response.Error
            });
        }

        [HttpPost]
        public ActionResult VotePost(Guid postId, VoteType type)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to vote for an item."
                });
            }

            _commandBus.Send(new CastVoteForPost
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
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

        public ActionResult UnVotePost(Guid postId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to unvote for an item."
                });
            }

            _commandBus.Send(new CastVoteForPost
            {
                UserId = _userContext.CurrentUser.Id,
                PostId = postId,
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

        [HttpPost]
        public ActionResult VoteComment(Guid commentId, VoteType type)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to vote for an item."
                });
            }

            _commandBus.Send(new CastVoteForComment
            {
                UserName = _userContext.CurrentUser.UserName,
                CommentId = commentId,
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

        public ActionResult UnVoteComment(Guid commentId)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to unvote for an item."
                });
            }

            _commandBus.Send(new CastVoteForComment
            {
                UserName = _userContext.CurrentUser.UserName,
                CommentId = commentId,
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

        public ActionResult ModerationSideBar(Guid subId)
        {
            var sub = _subDao.GetSubById(subId);

            if (sub == null)
                return new EmptyResult();

            var model = new ModerationSideBarModel();
            model.SubName = sub.Name;

            if (_userContext.CurrentUser != null)
            {
                model.IsModerator = _subDao.CanUserModerateSub(_userContext.CurrentUser.Id, sub.Id);
            }
            else
            {
                // we only show list of mods if the requesting user is not a mod of this sub
                model.Moderators.AddRange(_subDao.GetAllModsForSub(sub.Id));
            }

            return PartialView("_ModerationSideBar", model);
        }
    }
}