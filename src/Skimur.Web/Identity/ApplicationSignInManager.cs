using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Skimur.Web.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, Guid>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
    }
}