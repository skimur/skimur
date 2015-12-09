using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Services
{
    public interface IAvatarService
    {
        string UploadAvatar(IFormFile file, string key);

        Stream GetAvatarStream(string key);
    }
}
