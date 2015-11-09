
namespace Skimur.Web
{
    public class WebSettings : ISettings
    {
        public WebSettings()
        {
            DataDirectory = "~/Data/";
            ThumbnailCache = "~/ThumbnailCache";
        }

        public string Announcement { get; set; }

        public string DataDirectory { get; set; }

        public string ThumbnailCache { get; set; }
    }
}
