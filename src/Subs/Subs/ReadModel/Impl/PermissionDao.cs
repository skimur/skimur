using Subs.Services;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class PermissionDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : PermissionService, IPermissionDao
    {
        public PermissionDao(ISubService subService, ICommentService commentService)
            :base(subService, commentService)
        {
            
        }
    }
}
