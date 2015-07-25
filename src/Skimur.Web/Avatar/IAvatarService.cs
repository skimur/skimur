using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Avatar
{
    public interface IAvatarService
    {
        string UploadAvatar(HttpPostedFileBase file, string key);

        Stream GetAvatarStream(string key);
    }
}
