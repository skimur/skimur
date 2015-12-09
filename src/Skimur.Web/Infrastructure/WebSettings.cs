using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class WebSettings : ISettings
    {
        public WebSettings()
        {
            DataDirectory = Path.Combine("~", "Data");
            ThumbnailCache = Path.Combine("~", "ThumbnailCache");
        }

        public string Announcement { get; set; }

        public string DataDirectory { get; set; }

        public string ThumbnailCache { get; set; }
    }
}
