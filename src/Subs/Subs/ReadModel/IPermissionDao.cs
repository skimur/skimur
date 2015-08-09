using System;

namespace Subs.ReadModel
{
    public interface IPermissionDao
    {
        bool CanUserDeleteComment(Guid userId, Comment comment);

        bool CanUserMarkCommentAsSpam(Guid userId, Comment comment);

        bool CanUserMarkPostAsSpam(Guid userId, Post post);

        bool CanUserModerateSub(Guid userId, Guid subId);
    }
}
