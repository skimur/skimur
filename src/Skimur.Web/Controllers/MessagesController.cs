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

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class MessagesController : BaseController
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger<MessagesController> _logger;
        private readonly IUserContext _userContext;

        public MessagesController(ICommandBus commandBus, 
            ILogger<MessagesController> logger,
            IUserContext userContext)
        {
            _commandBus = commandBus;
            _logger = logger;
            _userContext = userContext;
        }

        public ActionResult Inbox()
        {
            ViewBag.ManageNavigationKey = "inbox";

            // TODO

            return View(new InboxViewModel());
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
