using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Web.Models
{
    public class ModerationSideBarModel
    {
        public ModerationSideBarModel()
        {
            Moderators = new List<Guid>();
        }

        public string SubName { get; set; }

        public bool IsModerator { get; set; }

        public List<Guid> Moderators { get; set; } 
    }
}
