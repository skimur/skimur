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

        IFileInfo GetFile(string file);
    }

    public interface IFileInfo
    {
        bool Exists { get; }

        void Open(FileMode mode, Action<Stream> action);

        Stream Open(FileMode mode);

        void Delete();
    }

    public interface IDirectoryInfo
    {
        string Path { get; }

        bool Exists { get; }

        void Create();

        void Delete();

        void Delete(bool recursive);

        IFileInfo GetFile(string file);
    }
}
