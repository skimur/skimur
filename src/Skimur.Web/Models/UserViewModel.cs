using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Skimur.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; }

        public bool IsModerator { get; set; }

        public List<string> ModeratingSubs { get; set; } 

        public int CommentKudos { get; set; }

        public int PostKudos { get; set; }
    }
}
