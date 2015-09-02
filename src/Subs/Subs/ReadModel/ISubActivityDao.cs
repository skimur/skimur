using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface ISubActivityDao
    {
        void MarkSubActive(Guid userId, Guid subId);

        int GetActiveNumberOfUsersForSub(Guid subId);

        int GetActiveNumberOfUsersForSubFuzzed(Guid subId, out bool wasActuallFuzzed);
    }
}
