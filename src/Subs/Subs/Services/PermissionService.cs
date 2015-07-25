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
        private readonly ICommentService _commentService;

        public PermissionService(ISubService subService, ICommentService commentService)
        {
            _subService = subService;
            _commentService = commentService;
        }

        public bool CanUserDeleteComment(string userName, Comment comment)
        {
            if (comment == null)
                return false;

            if (string.IsNullOrEmpty(userName))
                return false;

            // TODO: is user an admin?

            return _subService.CanUserModerateSub(userName, comment.SubName);
        }

        public bool CanUserDeleteComment(string userName, Guid commentId)
        {
            return CanUserDeleteComment(userName, _commentService.GetCommentById(commentId));
        }
    }
}
