using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.Services
{
    public interface IPermissionService
    {
        bool CanUserDeleteComment(User user, Comment comment);

        bool CanUserMarkCommentAsSpam(User user, Comment comment);

        bool CanUserMarkPostAsSpam(User user, Post post);
        
        bool CanUserManageSubAccess(User user, Guid subId);

        bool CanUserManageSubConfig(User user, Guid subId);

        bool CanUserManageSubFlair(User user, Guid subId);

        bool CanUserManageSubMail(User user, Guid subId);

        bool CanUserManageSubPosts(User user, Guid subId);

        bool CanUserManageSubStyles(User user, Guid subId);

        ModeratorPermissions? GetUserPermissionsForSub(User user, Guid subId);
    }
}
