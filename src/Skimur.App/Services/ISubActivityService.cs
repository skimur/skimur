using System;

namespace Skimur.App.Services
{
    public interface ISubActivityService
    {
        void MarkSubActive(Guid userId, Guid subId);

        int GetActiveNumberOfUsersForSub(Guid subId);
    }
}
