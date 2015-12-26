using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Skimur.FileSystem;
using System.Drawing;
using ImageResizer;
using System.Drawing.Imaging;

namespace Skimur.Web.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IPathResolver _pathResolver;
        private readonly IFileSystem _fileSystemProvider;
        private readonly IDirectoryInfo _avatarDirectoryInfo;

        public AvatarService(IPathResolver pathResolver, IFileSystem fileSystemProvider)
        {
            _pathResolver = pathResolver;
            _fileSystemProvider = fileSystemProvider;

            _avatarDirectoryInfo = _fileSystemProvider.GetDirectory("avatars");
            if (!_avatarDirectoryInfo.Exists)
                _avatarDirectoryInfo.Create();
        }

        public Stream GetAvatarStream(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var avatar = _avatarDirectoryInfo.GetFile(key + ".jpg");
            if (avatar.Exists)
                return avatar.Open(FileMode.Open);

            return null;
        }

        public string UploadAvatar(IFormFile file, string key)
        {
            if (file.Length >= 300000)
                throw new Exception("Uploaded image may not exceed 300 kb, please upload a smaller image.");

            try
            {
                using (var readStream = file.OpenReadStream())
                {
                    using (var img = Image.FromStream(readStream))
                    {
                        if (!img.RawFormat.Equals(ImageFormat.Jpeg) && !img.RawFormat.Equals(ImageFormat.Png))
                            throw new Exception("Uploaded file is not recognized as an image.");

                        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                        try
                        {
                            ImageBuilder.Current.Build(img, tempFile, new Instructions
                            {
                                Width = 150,
                                Height = 150,
                                Format = "jpg",
                                Mode = FitMode.Max
                            });

                            var avatar = _avatarDirectoryInfo.GetFile(key + ".jpg");
                            if (avatar.Exists)
                                avatar.Delete();
                            avatar.Open(FileMode.Create, stream =>
                            {
                                using (var imageStream = File.OpenRead(tempFile))
                                    imageStream.CopyTo(stream);
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Uploaded file is not recognized as a valid image.", ex);
                        }
                        finally
                        {
                            if (File.Exists(tempFile))
                                File.Delete(tempFile);
                        }

                        return key;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Uploaded file is not recognized as an image.");
            }
        }
    }
}
