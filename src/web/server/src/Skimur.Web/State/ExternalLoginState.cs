using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skimur.Web.State
{
    public class ExternalLoginState
    {
        public ExternalLoginState()
        {
            LoginProviders = new List<ExternalLoginProvider>();
        }

        [JsonProperty("loginProviders")]
        public List<ExternalLoginProvider> LoginProviders { get; set; }
        
        public class ExternalLoginProvider
        {
            [JsonProperty("scheme")]
            public string Scheme { get; set; }

            [JsonProperty("displayName")]
            public string DisplayName { get; set; }
        }
    }
}
