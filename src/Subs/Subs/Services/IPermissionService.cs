using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IPermissionService
    {
        bool CanUserDeleteComment(Guid userId, Comment comment);

        bool CanUserMarkCommentAsSpam(Guid userId, Comment comment);

        bool CanUserMarkPostAsSpam(Guid userId, Post post);

        bool CanUserModerateSub(Guid userId, Guid subId);
    }
}
