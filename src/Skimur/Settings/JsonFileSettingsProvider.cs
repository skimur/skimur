using System.IO;
using ServiceStack.Text;

namespace Skimur.Settings
{
    public class JsonFileSettingsProvider<T> : ISettingsProvider<T> where T : ISettings, new()
    {
        private FileSystemWatcher _watcher;
        private string _file;

        public JsonFileSettingsProvider(IPathResolver pathResolver)
        {
            _file = Path.Combine(pathResolver.Resolve(Path.Combine("~","Settings")), typeof(T).Name + ".json");
            Settings = File.Exists(_file) ? JsonSerializer.DeserializeFromString<T>(File.ReadAllText(_file)) : new T();
            if(Settings == null)
                Settings = new T();
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
            var parsed = File.Exists(_file) ? JsonSerializer.DeserializeFromString<T>(File.ReadAllText(_file)) : new T();
            if (parsed == null)
                parsed = new T();
            Settings = parsed;
        }

        public T Settings { get; private set; }
    }
}
