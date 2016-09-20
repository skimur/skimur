using Newtonsoft.Json;

namespace Skimur.Web.State
{
    public class GlobalState
    {
        public GlobalState()
        {
            Auth = new AuthState();
            ExternalLogins = new ExternalLoginState();
        }

        [JsonProperty("auth")]
        public AuthState Auth { get; set; }

        [JsonProperty("externalLogins")]
        public ExternalLoginState ExternalLogins { get; set; }
    }
}
