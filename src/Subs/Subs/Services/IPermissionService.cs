using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IPermissionService
    {
        bool CanUserDeleteComment(string userName, Comment comment);

        bool CanUserDeleteComment(string userName, Guid commentId);
    }
}
