using Newtonsoft.Json;
using Skimur.App;

namespace Skimur.Web.Models.Api
{
    public class ApiUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public static ApiUser From(User user)
        {
            return new ApiUser
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}
