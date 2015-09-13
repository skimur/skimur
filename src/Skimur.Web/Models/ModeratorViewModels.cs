using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;

namespace Skimur.Web.Models
{
    public class ModeratorsViewModel
    {
        public Sub Sub { get; set; }
        
        public List<ModeratorModel> Moderators { get; set; } 
    }

    public class ModeratorModel
    {
        public string UserName { get; set; }

        public DateTime DateJoined { get; set; }

        public ModeratorPermissions Permissions { get; set; }

        public bool PermissionsAll { get { return Permissions.HasFlag(ModeratorPermissions.All); } }

        public bool PermissionsAccess { get { return Permissions.HasFlag(ModeratorPermissions.Access); } }

        public bool PermissionsConfig { get { return Permissions.HasFlag(ModeratorPermissions.Config); } }

        public bool PermissionsFlair { get { return Permissions.HasFlag(ModeratorPermissions.Flair); } }

        public bool PermissionsMail { get { return Permissions.HasFlag(ModeratorPermissions.Mail); } }

        public bool PermissionsPosts { get { return Permissions.HasFlag(ModeratorPermissions.Posts); } }


    }
}
