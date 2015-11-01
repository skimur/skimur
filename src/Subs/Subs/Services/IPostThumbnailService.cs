using System.Drawing;
using System.IO;

namespace Subs.Services
{
    public interface IPostThumbnailService
    {
        string UploadImage(Image image);

        Stream GetImage(string thumb);
    }
}
