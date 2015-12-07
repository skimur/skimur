using Microsoft.AspNet.Mvc;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewComponents
{
    public class AccountHeaderViewComponent : ViewComponent
    {
        IUserContext _userContext;
        IMessageDao _messageDao;

        public AccountHeaderViewComponent(IUserContext userContext, IMessageDao messageDao)
        {
            _userContext = userContext;
            _messageDao = messageDao;
        }

        public IViewComponentResult Invoke()
        {
            var model = new AccountHeaderViewModel();
            model.CurrentUser = _userContext.CurrentUser;
            if (model.CurrentUser != null)
                model.NumberOfUnreadMessages = _messageDao.GetNumberOfUnreadMessagesForUser(model.CurrentUser.Id);
            return View(model);
        }
    }
}
