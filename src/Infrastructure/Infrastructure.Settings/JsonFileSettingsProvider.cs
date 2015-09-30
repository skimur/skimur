using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Infrastructure.Settings
{
    public class JsonFileSettingsProvider<T> : ISettingsProvider<T> where T : ISettings, new()
    {
        private FileSystemWatcher _watcher;
        private string _file;

        public JsonFileSettingsProvider(IPathResolver pathResolver)
        {
            _file = Path.Combine(pathResolver.Resolve("~/Settings/"), typeof(T).Name + ".json");
            Settings = File.Exists(_file) ? JsonSerializer.DeserializeFromString<T>(File.ReadAllText(_file)) : new T();
            if (File.Exists(_file))
            {
                _watcher = new FileSystemWatcher();
                _watcher.Path = Path.GetDirectoryName(_file);
                _watcher.Filter = Path.GetFileName(_file);
                _watcher.NotifyFilter = NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Size |
                    NotifyFilters.Security;
                _watcher.Changed += OnFileChanged;
                _watcher.EnableRaisingEvents = true;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Settings = File.Exists(_file) ? JsonSerializer.DeserializeFromString<T>(File.ReadAllText(_file)) : new T();
        }

        public T Settings { get; private set; }
    }
}
