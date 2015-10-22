using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        IDirectoryInfo GetDirectory(string directory);
    }

    public interface IFileInfo
    {
        
    }

    public interface IDirectoryInfo
    {
        bool Exists { get; }

        void Create();
    }

    class sdf
    {
        public sdf()
        {
            new DirectoryInfo();
        }
    }
}
