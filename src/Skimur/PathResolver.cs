using System;
using System.IO;
using System.Web.Hosting;

namespace Skimur
{
    public class PathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                if(Path.IsPathRooted(path))
                    return path;
                return HostingEnvironment.MapPath(path);
            }
                
            if (path.StartsWith("~/"))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(2));

            throw new NotSupportedException();
        }
    }
}
