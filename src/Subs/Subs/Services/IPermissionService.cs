using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IPermissionService
    {
        bool CanUserDeleteComment(string userName, Comment comment);

        bool CanUserMarkCommentAsSpam(string userName, Comment comment);

        bool CanUserMarkPostAsSpam(string userName, Post post);

        bool CanUserModerateSub(string userName, string subName);
    }
}
