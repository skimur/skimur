using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Subs.Services;

namespace Subs.ReadModel
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
