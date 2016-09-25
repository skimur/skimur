using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skimur.Web.State;
using Skimur.App;
using Skimur.Web.Models.Api;
using System.Collections.Generic;

namespace Skimur.Web.Controllers
{
    public class BaseController : Controller
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;

        public BaseController(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected async Task<GlobalState> BuildState()
        {
            var state = new GlobalState();

            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                state.Auth.User = ApiUser.From(user);
            }

            state.ExternalLogins.LoginProviders
                .AddRange(_signInManager.GetExternalAuthenticationSchemes()
                .Select(x => new ExternalLoginState.ExternalLoginProvider
                {
                    Scheme = x.AuthenticationScheme,
                    DisplayName = x.DisplayName
                }));

            return state;
        }

        protected async Task<User> GetCurrentUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return null;
            }

            return user;
        }

        protected IActionResult RedirectToLocal(string returnUrl)
        {
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
        }

        protected object GetModelState()
        {
            var result = new Dictionary<string, IList<string>>();
            foreach(var value in ModelState)
            {
                if(value.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    var key = ToCamelCase(value.Key);
                    if(string.IsNullOrEmpty(key))
                    {
                        key = "_global";
                    }
                    if (result.ContainsKey(key))
                    {
                        foreach(var error in value.Value.Errors)
                        {
                            result[key].Add(error.ErrorMessage);
                        }
                    }else
                    {
                        result.Add(key, value.Value.Errors.Select(x => x.ErrorMessage).ToList());
                    }
                }
            }
            return result;
        }

        protected string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            string camelCase = char.ToLower(s[0]).ToString();
            if (s.Length > 1)
                camelCase += s.Substring(1);

            return camelCase;
        }
    }
}
