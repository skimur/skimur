using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace Skimur.Web.Middleware
{
    public class RequestLogging : ISettings
    {
        public bool Enabled { get; set; }

        public List<int> ExcludeStatusCodes { get; set; } 
    }
}
