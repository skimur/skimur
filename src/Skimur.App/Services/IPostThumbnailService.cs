using System.Drawing;
using System.IO;

namespace Skimur.App.Services
{
    public interface IPostThumbnailService
    {
        string UploadImage(Image image);

        Stream GetImage(string thumb);
    }
}
