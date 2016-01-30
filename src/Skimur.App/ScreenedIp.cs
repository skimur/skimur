using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App
{
    public class ScreenedIp
    {
        public Guid Id { get; set; }

        public string Ip { get; set; }

        [Alias("Action")]
        public int ActionValue { get; set; }
        
        public ScreenedIpAction Action
        {
            get { return (ScreenedIpAction)ActionValue; }
            set { ActionValue = (int)value; }
        }

        public int NumberOfMatches { get; set; }

        public DateTime LastMatchAt { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }

    public enum ScreenedIpAction
    {
        /// <summary>
        /// This IP is to be blocked.
        /// </summary>
        Block = 0,
        /// <summary>
        /// This IP is whitelisted, and is verified friendly
        /// </summary>
        DoNothing = 1,
        /// <summary>
        /// This IP allows admin access
        /// </summary>
        AllowAdmin = 2
    }
}
