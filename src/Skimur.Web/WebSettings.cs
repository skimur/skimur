using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.Owin.Security;

namespace Skimur.Web
{
    public class WebSettings : ISettings
    {
        public WebSettings()
        {
            DataDirectory = "~/Data/";
        }

        public string Announcement { get; set; }

        public string DataDirectory { get; set; }
    }
}
