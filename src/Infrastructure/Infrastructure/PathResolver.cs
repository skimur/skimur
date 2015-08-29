using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Infrastructure
{
    public class PathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (HostingEnvironment.IsHosted)
                return HostingEnvironment.MapPath(path);

            if (path.StartsWith("~/"))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(2));

            throw new NotSupportedException();
        }
    }
}
