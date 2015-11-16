using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur.Logging;
using Skimur.Settings;

namespace Skimur.Embed
{
    public class ContextualEmbededProvider : IEmbeddedProvider
    {
        private readonly ISettingsProvider<EmbedSettings> _embedSettings;
        private readonly IEmbeddedProvider _embedlurProvider;

        public ContextualEmbededProvider(ISettingsProvider<EmbedSettings> embedSettings, ILogger<EmbedlurProvider> embedlurLogger)
        {
            _embedSettings = embedSettings;
            _embedlurProvider = new EmbedlurProvider(embedSettings, embedlurLogger);
        }

        public bool IsEnabled
        {
            get
            {
                if (_embedSettings.Settings.EmbedlurEnabled)
                    return true;
                return false;
            }
        }

        public IEmbeddedResult Embed(string url)
        {
            if (!IsEnabled) return null;

            return _embedlurProvider.Embed(url);
        }
    }
}
