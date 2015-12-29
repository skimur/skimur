using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App;

namespace Skimur.Web.ViewModels
{
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

    [Serializable]
    [Validator(typeof(StylesEditModelValidator))]
    public class StylesEditModel
    {
        public string Embedded { get; set; }

        [Display(Name="External CSS")]
        public string ExternalCss { get; set; }

        [Display(Name ="Project name")]
        public string GitHubCssProjectName { get; set; }

        [Display(Name ="Project tag")]
        public string GitHubCssProjectTag { get; set; }

        [Display(Name ="Project name")]
        public string GitHubLessProjectName { get; set; }

        [Display(Name ="Project tag")]
        public string GitHubLessProjectTag { get; set; }

        public CssType CssType { get; set; }

        public Sub Sub { get; set; }

        public bool StyledEnabledForUser { get; set; }
    }

    public class StylesEditModelValidator : AbstractValidator<StylesEditModel>
    {
        private static string _urlSafeStringRegex = @"[a-zA-Z0-9_\.-]";
        
        public StylesEditModelValidator()
        {
            RuleFor(x => x.Embedded)
                .Length(0, 50000)
                .WithMessage("Embedded styles can only be a maximum of 50,000 characters.");

            RuleFor(x => x.ExternalCss).Length(0, 300).WithMessage("The external css cannot be more than 300 characters in length.");
            RuleFor(x => x.ExternalCss)
                .Matches(@"^((https)://)([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$")
                .WithMessage("The HTTPS url doesn't appear to be valid.");

            RuleFor(x => x.GitHubCssProjectName)
                .Matches(_urlSafeStringRegex + @"{1,100}\/" + _urlSafeStringRegex + @"{1,100}")
                .WithMessage("The project name must be in \"user/repo\" format.");
            RuleFor(x => x.GitHubCssProjectName).NotEmpty().When(x => !string.IsNullOrEmpty(x.GitHubCssProjectTag))
                .WithMessage("The project name is required.");

            RuleFor(x => x.GitHubCssProjectTag)
                .Matches(_urlSafeStringRegex + @"{1,100}")
                .WithMessage("The tag is in an invalid format.");
            RuleFor(x => x.GitHubCssProjectTag).NotEmpty().When(x => !string.IsNullOrEmpty(x.GitHubCssProjectName))
                .WithMessage("The tag is required.");

            RuleFor(x => x.GitHubLessProjectName)
              .Matches(_urlSafeStringRegex + @"{1,100}\/" + _urlSafeStringRegex + @"{1,100}")
              .WithMessage("The project name must be in \"user/repo\" format.");
            RuleFor(x => x.GitHubLessProjectName).NotEmpty().When(x => !string.IsNullOrEmpty(x.GitHubLessProjectTag))
                .WithMessage("The project name is required.");

            RuleFor(x => x.GitHubLessProjectTag)
                .Matches(_urlSafeStringRegex + @"{1,100}")
                .WithMessage("The tag is in an invalid format.");
            RuleFor(x => x.GitHubLessProjectTag).NotEmpty().When(x => !string.IsNullOrEmpty(x.GitHubLessProjectName))
                .WithMessage("The tag is required.");
        }
    }
}
