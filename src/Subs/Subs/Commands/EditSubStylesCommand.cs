using System;
using Skimur.Messaging;

namespace Subs.Commands
{
    public class EditSubStylesCommand : ICommandReturns<EditSubStylesCommandResponse>
    {
        public Guid EditedByUserId { get; set; }

        public string SubName { get; set; }

        public Guid? SubId { get; set; }
        
        public CssType CssType { get; set; }

        public string Embedded { get; set; }

        public string ExternalCss { get; set; }
        
        public string GitHubCssProjectName { get; set; }
        
        public string GitHubCssProjectTag { get; set; }
        
        public string GitHubLessProjectName { get; set; }
        
        public string GitHubLessProjectTag { get; set; }
    }

    public class EditSubStylesCommandResponse
    {
        public string Error { get; set; }
    }
}
