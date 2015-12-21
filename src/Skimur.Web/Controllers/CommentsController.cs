using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Skimur.Messaging;
using Skimur.Web.Infrastructure;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs.Commands;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Controllers
{
    public class CommentsController : BaseController
    {
        private readonly ICommandBus _commandBus;
        private readonly IUserContext _userContext;
        private readonly ICommentWrapper _commentWrapper;
        private readonly ICommentTreeContextBuilder _commentTreeContextBuilder;
        private readonly ICommentNodeHierarchyBuilder _commentNodeHierarchyBuilder;
        private readonly ICommentDao _commentDao;

        public CommentsController(ICommandBus commandBus,
            IUserContext userContext,
            ICommentWrapper commentWrapper,
            ICommentTreeContextBuilder commentTreeContextBuilder,
            ICommentNodeHierarchyBuilder commentNodeHierarchyBuilder,
            ICommentDao commentDao)
        {
            _commandBus = commandBus;
            _userContext = userContext;
            _commentWrapper = commentWrapper;
            _commentTreeContextBuilder = commentTreeContextBuilder;
            _commentNodeHierarchyBuilder = commentNodeHierarchyBuilder;
            _commentDao = commentDao;
        }

        [Authorize, Ajax, HttpPost]
        public ActionResult Create(CreateCommentModel model)
        {
            var dateCreated = Common.CurrentTime();
            var response = _commandBus.Send<CreateComment, CreateCommentResponse>(new CreateComment
            {
                PostId = model.PostId,
                ParentId = model.ParentId,
                DateCreated = dateCreated,
                AuthorIpAddress = HttpContext.RemoteAddress(),
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

        [Authorize, Ajax, HttpPost]
        public ActionResult Edit(EditCommentModel model)
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

        [Authorize, Ajax, HttpPost]
        public ActionResult Delete(Guid commentId)
        {
            var response = _commandBus.Send<DeleteComment, DeleteCommentResponse>(new DeleteComment
            {
                CommentId = commentId,
                UserName = _userContext.CurrentUser.UserName,
                DateDeleted = Common.CurrentTime()
            });

            return CommonJsonResult(response.Error);
        }

        [Ajax, HttpPost]
        public ActionResult More(Guid postId, CommentSortBy? sort, string children, int depth)
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
    }
}
