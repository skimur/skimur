using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skimur.Web.Models;
using Skimur.App;

namespace Skimur.Web.Controllers.Status
{
    public class StatusController : BaseController
    {
        public StatusController(UserManager<User> userManager,
            SignInManager<User> signInManager)
            :base(userManager, signInManager)
        {
        }

        [Route("status/status/{statusCode}")]
        public async Task<IActionResult> Status(int statusCode)
        {
            return View($"js-/statuscode{statusCode}", await BuildState());
        }
    }
}
