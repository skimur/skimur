using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;

namespace Skimur.Web.Models
{
    public class StylesEditModel
    {
        public string Embedded { get; set; }

        public string ExternalCss { get; set; }
        
        public string GitHubCssProjectName { get; set; }
        
        public string GitHubCssProjectTag { get; set; }
        
        public string GitHubLessProjectName { get; set; }
        
        public string GitHubLessProjectTag { get; set; }
        
        public CssType CssType { get; set; }

        public Sub Sub { get; set; }
    }
}
