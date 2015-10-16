using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Skimur.Web.Models
{
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
