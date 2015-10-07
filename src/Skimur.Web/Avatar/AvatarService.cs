using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImageResizer;
using Infrastructure;
using Infrastructure.Settings;

namespace Skimur.Web.Avatar
{
    public class AvatarService : IAvatarService
    {
        private readonly IPathResolver _pathResolver;
        private string _avatarDirectory;

        public AvatarService(ISettingsProvider<WebSettings> webSettings, IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
            _avatarDirectory = _pathResolver.Resolve(webSettings.Settings.AvatarDirectory);
            if (!Directory.Exists(_avatarDirectory))
                Directory.CreateDirectory(_avatarDirectory);
        }

        public string UploadAvatar(HttpPostedFileBase file, string key)
        {
            if (file.ContentLength >= 300000)
                throw new Exception("Uploaded image may not exceed 300 kb, please upload a smaller image.");

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    if (!img.RawFormat.Equals(ImageFormat.Jpeg) && !img.RawFormat.Equals(ImageFormat.Png))
                        throw new Exception("Uploaded file is not recognized as an image.");

                    var newKeyPath = GetAvatarPathByIdentifier(key);

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

                        File.Copy(tempFile, newKeyPath, true);
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
            catch (Exception)
            {
                throw new Exception("Uploaded file is not recognized as an image.");
            }
        }

        public Stream GetAvatarStream(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var path = GetAvatarPathByIdentifier(key);

            if (File.Exists(path))
                return File.OpenRead(path);

            return null;
        }

        private string GetAvatarPathByIdentifier(string identifier)
        {
            return Path.Combine(_avatarDirectory, identifier + ".jpg");
        }
    }
}
