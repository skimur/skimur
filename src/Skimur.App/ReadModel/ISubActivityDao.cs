using System;

namespace Skimur.App.ReadModel
{
    public interface ISubActivityDao
    {
        void MarkSubActive(Guid userId, Guid subId);

        int GetActiveNumberOfUsersForSub(Guid subId);

        int GetActiveNumberOfUsersForSubFuzzed(Guid subId, out bool wasActuallFuzzed);
    }
}
