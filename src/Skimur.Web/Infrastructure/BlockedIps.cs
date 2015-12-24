using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class BlockedIps : ISettings
    {
        public List<string> Ips { get; set; }
    }
}
