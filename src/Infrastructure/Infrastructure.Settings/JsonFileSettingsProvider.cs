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
        public JsonFileSettingsProvider(IPathResolver pathResolver)
        {
            var settingsFile = Path.Combine(pathResolver.Resolve("~/Settings/"), typeof (T).Name + ".json");
            Settings = File.Exists(settingsFile) ? JsonSerializer.DeserializeFromString<T>(File.ReadAllText(settingsFile)) : new T();
        }

        public T Settings { get; private set; }
    }
}
