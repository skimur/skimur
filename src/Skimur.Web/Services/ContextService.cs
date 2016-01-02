using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App.ReadModel;

namespace Skimur.Web.Services
{
    public class ContextService : IContextService
    {
        private readonly ISubDao _subDao;
        private readonly IUserContext _userContext;

        public ContextService(ISubDao subDao, IUserContext userContext)
        {
            _subDao = subDao;
            _userContext = userContext;
        }

        public List<Guid> GetSubscribedSubIds()
        {
            // todo: optimize
            return _userContext.CurrentUser != null ? _subDao.GetSubscribedSubsForUser(_userContext.CurrentUser.Id) : _subDao.GetDefaultSubs();
        }

        public bool IsSubcribedToSub(Guid subId)
        {
            // todo: optimize
            return _userContext.CurrentUser == null ? _subDao.GetDefaultSubs().Any(x => x == subId) : _subDao.IsUserSubscribedToSub(_userContext.CurrentUser.Id, subId);
        }
    }
}
