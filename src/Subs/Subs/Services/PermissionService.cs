using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ISubService _subService;

        public PermissionService(ISubService subService, ICommentService commentService)
        {
            _subService = subService;
        }

        public bool CanUserDeleteComment(string userName, Comment comment)
        {
            if (comment.AuthorUserName == userName)
                return true;

            // TODO: is user an admin?

            return _subService.CanUserModerateSub(userName, comment.SubName);
        }

        public bool CanUserMarkCommentAsSpam(string userName, Comment comment)
        {
            return CanUserModerateSub(userName, comment.SubName);
        }

        public bool CanUserMarkPostAsSpam(string userName, Post post)
        {
            return CanUserModerateSub(userName, post.SubName);
        }

        public bool CanUserModerateSub(string userName, string subName)
        {
            return _subService.CanUserModerateSub(userName, subName);
        }
    }
}
