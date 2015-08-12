using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel
{
    public class SubUserBanWrapped
    {
        public SubUserBanWrapped(SubUserBan userBan)
        {
            Item = userBan;
        }

        public SubUserBan Item { get; set; }

        public User User { get; set; }

        public User BannedBy { get; set; }
    }
}
