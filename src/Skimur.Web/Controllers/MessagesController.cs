using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Skimur.Web.Models;
using Subs.Commands;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class MessagesController : BaseController
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger<MessagesController> _logger;
        private readonly IUserContext _userContext;
        private readonly IMessageDao _messageDao;
        private readonly IMessageWrapper _messageWrapper;

        public MessagesController(ICommandBus commandBus,
            ILogger<MessagesController> logger,
            IUserContext userContext,
            IMessageDao messageDao,
            IMessageWrapper messageWrapper)
        {
            _commandBus = commandBus;
            _logger = logger;
            _userContext = userContext;
            _messageDao = messageDao;
            _messageWrapper = messageWrapper;
        }

        public ActionResult Inbox(InboxType type, int? pageNumber, int? pageSize)
        {
            ViewBag.ManageNavigationKey = "inbox";

            if (pageNumber == null || pageNumber < 1)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 25;
            if (pageSize > 100)
                pageSize = 100;
            if (pageSize < 1)
                pageSize = 1;

            var skip = (pageNumber - 1)*pageSize;
            var take = pageSize;

            SeekedList<Guid> messages;

            switch (type)
            {
                case InboxType.All:
                    messages = _messageDao.GetAllMessagesForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                case InboxType.Unread:
                    messages = _messageDao.GetUnreadMessagesForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                case InboxType.Messages:
                    messages = _messageDao.GetPrivateMessagesForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                case InboxType.CommentReplies:
                    messages = _messageDao.GetCommentRepliesForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                case InboxType.PostReplies:
                    messages = _messageDao.GetPostRepliesForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                case InboxType.Mentions:
                    messages = _messageDao.GetMentionsForUser(_userContext.CurrentUser.Id, skip, take);
                    break;
                default:
                    throw new Exception("Unknown inbox type");
            }

            var model = new InboxViewModel();
            model.InboxType = type;
            model.Messages = new PagedList<MessageWrapped>(_messageWrapper.Wrap(messages, _userContext.CurrentUser), 0, 30, messages.HasMore);

            return View(model);
        }

        public ActionResult Compose(string to = null, string subject = null, string message = null)
        {
            ViewBag.ManageNavigationKey = "compose";

            ModelState.Clear(); // prevent query string from overriding our model properties

            var model = new ComposeMessageViewModel();
            model.To = to;
            model.Subject = subject;
            model.Message = message;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compose(ComposeMessageViewModel model)
        {
            ViewBag.ManageNavigationKey = "compose";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SendMessageResponse response = null;

            try
            {
                response = _commandBus.Send<SendMessage, SendMessageResponse>(new SendMessage
                {
                    To = model.To,
                    Subject = model.Subject,
                    Body = model.Message,
                    Author = _userContext.CurrentUser.Id
                });
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured sending a message.", ex);
                AddErrorMessage("An unknown error occured.");
            }

            if (string.IsNullOrEmpty(response.Error))
            {
                AddSuccessMessage("Your message has been sent.");
            }
            else
            {
                AddErrorMessage(response.Error);
            }

            return View(model);
        }
    }
}
