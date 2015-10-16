using System;
using Membership;

namespace Subs.Services.Impl
{
    public class PermissionService : IPermissionService
    {
        private readonly IModerationService _moderationService;

        public PermissionService(IModerationService moderationService)
        {
            _moderationService = moderationService;
        }

        public bool CanUserDeleteComment(User user, Comment comment)
        {
            if (user == null) return false;

            if (comment.AuthorUserId == user.Id)
                return true;
            
            return CanUserManageSubPosts(user, comment.SubId);
        }

        public bool CanUserMarkCommentAsSpam(User user, Comment comment)
        {
            return CanUserManageSubPosts(user, comment.SubId);
        }

        public bool CanUserMarkPostAsSpam(User user, Post post)
        {
            return CanUserManageSubPosts(user, post.SubId);
        }

        public bool CanUserManageSubAccess(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Access);
        }

        public bool CanUserManageSubConfig(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Config);
        }

        public bool CanUserManageSubFlair(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Flair);
        }

        public bool CanUserManageSubMail(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Mail);
        }

        public bool CanUserManageSubPosts(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Posts);
        }

        public bool CanUserManageSubStyles(User user, Guid subId)
        {
            var permissions = GetUserPermissionsForSub(user, subId);
            if (permissions == null) return false;
            return permissions.HasPermission(ModeratorPermissions.Styles);
        }

        public ModeratorPermissions? GetUserPermissionsForSub(User user, Guid subId)
        {
            return _moderationService.GetUserPermissionsForSub(user, subId);
        }
    }
}
