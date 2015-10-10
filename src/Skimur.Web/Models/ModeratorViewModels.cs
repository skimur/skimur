using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class ModeratorsViewModel
    {
        public Sub Sub { get; set; }
        
        public List<ModeratorWrapped> Moderators { get; set; } 

        public List<ModeratorInviteWrapped> Invites { get; set; } 

        public ModeratorInvite CurrentUserInvite { get; set; }

        public bool CanInvite { get; set; }
    }
}
