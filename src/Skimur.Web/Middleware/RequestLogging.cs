using System.Collections.Generic;

namespace Skimur.Web.Middleware
{
    public class RequestLogging : ISettings
    {
        public bool Enabled { get; set; }

        public List<int> ExcludeStatusCodes { get; set; } 
    }
}
