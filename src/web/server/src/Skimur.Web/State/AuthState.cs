using Newtonsoft.Json;
using Skimur.Web.Models.Api;

namespace Skimur.Web.State
{
    public class AuthState
    {
        [JsonProperty("user")]
        public ApiUser User { get; set; }

        [JsonProperty("loggedIn")]
        public bool LoggedIn { get; set; }
    }
}
