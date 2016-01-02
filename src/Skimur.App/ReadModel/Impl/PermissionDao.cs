using Skimur.App.Services;
using Skimur.App.Services.Impl;

namespace Skimur.App.ReadModel.Impl
{
    public class PermissionDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : PermissionService, IPermissionDao
    {
        public PermissionDao(IModerationService moderationService)
            :base(moderationService)
        {
            
        }
    }
}
