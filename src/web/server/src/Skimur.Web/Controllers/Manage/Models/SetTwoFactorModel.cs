using Newtonsoft.Json;

namespace Skimur.Web.Controllers.Manage.Models
{
    public class SetTwoFactorModel
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
