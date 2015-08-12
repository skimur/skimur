using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel.Impl
{
    public class SubUserBanWrapper : ISubUserBanWrapper
    {
        private readonly IMembershipService _membershipService;

        public SubUserBanWrapper(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public List<SubUserBanWrapped> Wrap(List<SubUserBan> items)
        {
            var users =
                _membershipService.GetUsersByIds(
                    items.Select(x => x.UserId).Union(items.Select(x => x.BannedBy)).Distinct().ToList())
                    .ToDictionary(x => x.Id, x => x);

            return items.Select(x =>
                {
                    var wrapped = new SubUserBanWrapped(x)
                    {
                        User = users.ContainsKey(x.UserId) ? users[x.UserId] : null,
                        BannedBy = users.ContainsKey(x.BannedBy) ? users[x.BannedBy] : null
                    };
                    return wrapped;
                })
                .ToList();
        }
    }
}
