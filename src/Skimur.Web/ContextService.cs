using System.Collections.Generic;
using System.Linq;
using Subs.ReadModel;

namespace Skimur.Web
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

        public List<string> GetSubscribedSubNames()
        {
            // todo: optimize
            return _userContext.CurrentUser != null ? _subDao.GetSubscribedSubsForUser(_userContext.CurrentUser.UserName).Select(x => x.Name).ToList() : _subDao.GetDefaultSubs().Select(x => x.Name).ToList();
        }

        public bool IsSubcribedToSub(string subName)
        {
            // todo: optimize
            return _userContext.CurrentUser == null ? _subDao.GetDefaultSubs().Any(x => x.Name == subName) : _subDao.GetSubscribedSubsForUser(_userContext.CurrentUser.UserName).Any(x => x.Name == subName);
        }
    }
}
