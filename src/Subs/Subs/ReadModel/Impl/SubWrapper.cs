using System;
using System.Collections.Generic;
using Infrastructure.Membership;

namespace Subs.ReadModel.Impl
{
    public class SubWrapper : ISubWrapper
    {
        private readonly ISubDao _subDao;
        private readonly IVoteDao _voteDao;

        public SubWrapper(ISubDao subDao, IVoteDao voteDao)
        {
            _subDao = subDao;
            _voteDao = voteDao;
        }

        public List<SubWrapped> Wrap(List<Guid> subIds, User currentUser = null)
        {
            var subs = new List<SubWrapped>();
            foreach (var subId in subIds)
            {
                var sub = _subDao.GetSubById(subId);
                if (sub != null)
                    subs.Add(new SubWrapped(sub));
            }
            
            var subscribed = currentUser != null ? _subDao.GetSubscribedSubsForUser(currentUser.Id) : new List<Guid>();

            foreach (var item in subs)
            {
                if (currentUser != null)
                    item.IsSubscribed = subscribed.Contains(item.Sub.Id);
            }

            return subs;
        }

        public SubWrapped Wrap(Guid subId, User currentUser = null)
        {
            return Wrap(new List<Guid> {subId})[0];
        }
    }
}
