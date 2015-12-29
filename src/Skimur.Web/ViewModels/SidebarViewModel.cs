using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App;
using Skimur.App.ReadModel;

namespace Skimur.Web.ViewModels
{
    public class SidebarViewModel
    {
        public SubWrapped CurrentSub { get; set; }

        public int? NumberOfActiveUsers { get; set; }

        public bool IsNumberOfActiveUsersFuzzed { get; set; }

        public bool IsModerator { get { return Permissions.HasValue; } }

        public ModeratorPermissions? Permissions { get; set; }

        public List<User> Moderators { get; set; }

        public bool ShowSubmit { get; set; }

        public bool ShowCreateSub { get; set; }

        public bool CanCreateSub { get; set; }

        public bool ShowSearch { get; set; }
    }
}
