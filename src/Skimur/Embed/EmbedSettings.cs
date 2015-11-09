using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Embed
{
    public class EmbedSettings : ISettings
    {
        public EmbedSettings()
        {
            EmbedlurEnabled = true;
            EmbedlurEndpoint = "http://localhost:35838/";
        }

        public bool EmbedlurEnabled { get; set; }

        public string EmbedlurEndpoint { get; set; }
    }
}
