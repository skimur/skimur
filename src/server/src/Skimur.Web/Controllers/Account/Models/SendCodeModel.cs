using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Skimur.Web.Controllers.Account.Models
{
    public class SendCodeModel
    {
        [Required]
        [JsonProperty("provider")]
        public string Provider { get; set; }
    }
}
