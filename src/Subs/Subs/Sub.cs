using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("Subs")]
    public class Sub
    {
        public virtual Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string SidebarText { get; set; }

        public string SidebarTextFormatted { get; set; }

        public string SubmissionText { get; set; }

        public string SubmissionTextFormatted { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public int NumberOfSubscribers { get; set; }

        public int Type { get; set; }

        public Guid? CreatedBy { get; set; }

        public bool Nsfw { get; set; }

        public bool InAll { get; set; }

        [Ignore]
        public SubType SubType
        {
            get { return (SubType) Type; }
            set { Type = (int)value; }
        }
    }

    public enum SubType
    {
        Public,
        Restricted,
        Private
    }
}
