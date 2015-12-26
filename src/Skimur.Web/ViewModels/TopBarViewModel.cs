using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
{
    public class TopBarViewModel
    {
        public TopBarViewModel()
        {
            SubscibedSubs = new List<string>();
            DefaultSubs = new List<string>();
        }

        public List<string> SubscibedSubs { get; set; }

        public List<string> DefaultSubs { get; set; }
    }
}
