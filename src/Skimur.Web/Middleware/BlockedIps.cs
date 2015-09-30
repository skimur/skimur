using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace Skimur.Web.Middleware
{
    public class BlockedIps : ISettings
    {
        public List<string> Ips { get; set; } 
    }
}
