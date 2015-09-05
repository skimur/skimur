using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skimur.Web.Models;
using Subs.ReadModel;

namespace Skimur.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IContextService _contextService;
        private readonly ISubDao _subDao;
        private readonly IUserContext _userContext;
        private readonly IMessageDao _messageDao;

        public CommonController(IContextService contextService,
            ISubDao subDao,
            IUserContext userContext,
            IMessageDao messageDao)
        {
            _contextService = contextService;
            _subDao = subDao;
            _userContext = userContext;
            _messageDao = messageDao;
        }
        
        public ActionResult TopBar()
        {
            var model = new TopBarViewModel();
            model.SubscibedSubs.AddRange(_subDao.GetSubsByIds(_contextService.GetSubscribedSubIds()).Select(x => x.Name));
            return PartialView(model); 
        }

        public ActionResult Account()
        {
            var model = new AccountViewModel();
            model.CurrentUser = _userContext.CurrentUser;
            if (model.CurrentUser != null)
                model.NumberOfUnreadMessages = _messageDao.GetNumberOfUnreadMessagesForUser(model.CurrentUser.Id);
            return PartialView(model);
        }
    }
}
