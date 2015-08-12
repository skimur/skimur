using System;

namespace Subs.Services.Impl
{
    public class PermissionService : IPermissionService
    {
        private readonly ISubService _subService;

        public PermissionService(ISubService subService, ICommentService commentService)
        {
            _subService = subService;
        }

        public bool CanUserDeleteComment(Guid userId, Comment comment)
        {
            if (comment.AuthorUserId == userId)
                return true;

            // TODO: is user an admin?

            return _subService.CanUserModerateSub(userId, comment.SubId);
        }

        public bool CanUserMarkCommentAsSpam(Guid userId, Comment comment)
        {
            return CanUserModerateSub(userId, comment.SubId);
        }

        public bool CanUserMarkPostAsSpam(Guid userId, Post post)
        {
            return CanUserModerateSub(userId, post.SubId);
        }

        public bool CanUserModerateSub(Guid userId, Guid subId)
        {
            return _subService.CanUserModerateSub(userId, subId);
        }
    }
}
