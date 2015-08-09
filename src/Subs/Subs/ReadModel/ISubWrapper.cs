using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

namespace Subs.ReadModel
{
    public interface ISubWrapper
    {
        List<SubWrapped> Wrap(List<Guid> subIds, User currentUser = null);

        SubWrapped Wrap(Guid subId, User currentUser = null);
    }
}
