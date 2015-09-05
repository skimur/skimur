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

        bool CanUserModerateSub(User user, Guid subId);
    }
}
