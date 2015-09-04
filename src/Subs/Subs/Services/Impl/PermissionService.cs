using System;
using Membership;

namespace Subs.Services.Impl
{
    public class PermissionService : IPermissionService
    {
        private readonly ISubService _subService;

        public PermissionService(ISubService subService)
        {
            _subService = subService;
        }

        public bool CanUserDeleteComment(User user, Comment comment)
        {
            if (user == null) return false;

            if (comment.AuthorUserId == user.Id)
                return true;

            if (user.IsAdmin) return true;
            
            return CanUserModerateSub(user, comment.SubId);
        }

        public bool CanUserMarkCommentAsSpam(User user, Comment comment)
        {
            return CanUserModerateSub(user, comment.SubId);
        }

        public bool CanUserMarkPostAsSpam(User user, Post post)
        {
            return CanUserModerateSub(user, post.SubId);
        }

        public bool CanUserModerateSub(User user, Guid subId)
        {
            if (user.IsAdmin) return true;

            return _subService.CanUserModerateSub(user.Id, subId);
        }
    }
}
