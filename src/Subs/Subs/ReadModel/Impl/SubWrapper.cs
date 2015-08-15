using System;
using System.Collections.Generic;
using System.Linq;
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
            var subs = subIds.Select(subId => _subDao.GetSubById(subId)).Where(sub => sub != null).ToList();
            return Wrap(subs, currentUser);
        }

        public SubWrapped Wrap(Guid subId, User currentUser = null)
        {
            return Wrap(new List<Guid> {subId}, currentUser)[0];
        }

        public List<SubWrapped> Wrap(List<Sub> subs, User currentUser = null)
        {
            var wrapped = new List<SubWrapped>();
            foreach (var sub in subs)
            {
                wrapped.Add(new SubWrapped(sub));
            }

            var subscribed = currentUser != null ? _subDao.GetSubscribedSubsForUser(currentUser.Id) : new List<Guid>();

            foreach (var item in wrapped)
            {
                if (currentUser != null)
                    item.IsSubscribed = subscribed.Contains(item.Sub.Id);
            }

            return wrapped;
        }

        public SubWrapped Wrap(Sub sub, User currentUser = null)
        {
            return Wrap(new List<Sub> { sub })[0];
        }
    }
}
