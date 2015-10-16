using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;
using FluentValidation.Attributes;

namespace Skimur.Web.Models
{
    [Serializable]
    [Validator(typeof(StylesEditModelValidator))]
    public class StylesEditModel
    {
        public string Embedded { get; set; }

        [DisplayName("External CSS")]
        public string ExternalCss { get; set; }

        [DisplayName("Project name")]
        public string GitHubCssProjectName { get; set; }

        [DisplayName("Project tag")]
        public string GitHubCssProjectTag { get; set; }

        [DisplayName("Project name")]
        public string GitHubLessProjectName { get; set; }

        [DisplayName("Project tag")]
        public string GitHubLessProjectTag { get; set; }
        
        public CssType CssType { get; set; }
        
        public Sub Sub { get; set; }

        public bool StyledEnabledForUser { get; set; }
    }

    [Serializable]
    public class StylesPreviewModel
    {
        public string Embedded { get; set; }

        public string ExternalCss { get; set; }

        public string GitHubCssProjectName { get; set; }

        public string GitHubCssProjectTag { get; set; }

        public string GitHubLessProjectName { get; set; }

        public string GitHubLessProjectTag { get; set; }

        public CssType CssType { get; set; }

        public DateTime ExpiresOn { get; set; }
    }
}
