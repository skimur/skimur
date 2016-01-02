using System;
using Skimur.Messaging;

namespace Skimur.App.Commands
{
    public class EditSub : ICommandReturns<EditSubResponse>
    {
        public Guid EditedByUserId { get; set; }

        public string Name { get; set; }

        public string SidebarText { get; set; }

        public string SubmissionText { get; set; }

        public string Description { get; set; }

        public bool? IsDefault { get; set; }

        public SubType Type { get; set; }

        public bool InAll { get; set; }

        public bool Nsfw { get; set; }
    }

    public class EditSubResponse
    {
        public string Error { get; set; }
    }
}
