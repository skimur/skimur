using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace Skimur.Web
{
    public class WebSettings : ISettings
    {
        public WebSettings()
        {
            AvatarDirectory = "~/ Avatars";
        }

        public string AvatarDirectory { get; set; }
    }
}
