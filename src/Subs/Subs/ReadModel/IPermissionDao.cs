using System;
using Membership;

namespace Subs.ReadModel
{
    public interface IPermissionDao
    {
        bool CanUserDeleteComment(User user, Comment comment);

        bool CanUserMarkCommentAsSpam(User user, Comment comment);

        bool CanUserMarkPostAsSpam(User user, Post post);

        bool CanUserModerateSub(User user, Guid subId);
    }
}
