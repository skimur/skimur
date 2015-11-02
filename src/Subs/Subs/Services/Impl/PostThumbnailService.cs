using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Infrastructure.FileSystem;

namespace Subs.Services.Impl
{
    public class PostThumbnailService : IPostThumbnailService
    {
        readonly IDirectoryInfo _thumbnailDirectory;

        public PostThumbnailService(IFileSystem fileSystem)
        {
            _thumbnailDirectory = fileSystem.GetDirectory("post-thumbs");
            if (!_thumbnailDirectory.Exists)
                _thumbnailDirectory.Create();
        }

        public string UploadImage(Image image)
        {
            // TODO: maybe a global lock while we upload this image?

            var fileName = RandomKey() + ".jpg";
            while (_thumbnailDirectory.FileExists(fileName))
                fileName = RandomKey() + ".jpg";

            try
            {
                _thumbnailDirectory.GetFile(fileName).Open(FileMode.Create, stream => image.Save(stream, ImageFormat.Jpeg));
            }
            catch (Exception)
            {
                try
                {
                    // if there was any issue, try to delete the file, if we created
                    if (_thumbnailDirectory.FileExists(fileName))
                        _thumbnailDirectory.DeleteFile(fileName);
                }
                catch (Exception)
                {
                    // the error was more fundamental.
                    // don't do anything with this exception.
                    // let the previous exception be the real one thrown.
                }
                throw;
            }

            return fileName;
        }

        public Stream GetImage(string thumb)
        {
            var file = _thumbnailDirectory.GetFile(thumb);
            if (!file.Exists) return null;
            return file.Open(FileMode.Open);
        }

        private string RandomKey()
        {
            var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            key = key.Replace("/", "_");
            key = key.Replace("+", "-");
            return key.Substring(0, 22);
        }
    }
}
