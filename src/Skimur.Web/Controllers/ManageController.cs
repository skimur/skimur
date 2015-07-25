using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Membership;
using Infrastructure.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Skimur.Web.Avatar;
using Skimur.Web.Identity;
using Skimur.Web.Models;

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUserContext _userContext;
        private readonly IMembershipService _membershipService;
        private readonly IAvatarService _avatarService;
        private ApplicationUserManager _userManager;

        public ManageController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authenticationManager,
            IUserContext userContext,
            IMembershipService membershipService,
            IAvatarService avatarService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _userContext = userContext;
            _membershipService = membershipService;
            _avatarService = avatarService;
        }

        [ActionName("Index")]
        public async Task<ActionResult> ManageProfile()
        {
            ViewBag.ManageNavigationKey = "Profile";

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ParseGuid());

            return View(new ProfileViewModel
            {
                AvatarIdentifier = user.AvatarIdentifier,
                FullName = user.FullName,
                Bio = user.Bio,
                Url = user.Url,
                Location = user.Location
            });
        }

        [ActionName("Index")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProfile(ProfileViewModel model)
        {
            ViewBag.ManageNavigationKey = "Profile";

            if (!ModelState.IsValid)
                return View(model);

            var user = _membershipService.GetUserById(User.Identity.GetUserId().ParseGuid());

            if (model.AvatarFile != null)
            {
                try
                {
                    var avatarKey = _avatarService.UploadAvatar(model.AvatarFile, user.UserName);
                    _membershipService.UpdateUserAvatar(user.Id, avatarKey);
                    model.AvatarIdentifier = avatarKey;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }

            _membershipService.UpdateUserProfile(User.Identity.GetUserId().ParseGuid(), model.FullName, model.Bio, model.Url, model.Location);

            AddSuccessMessage("Your profile has been updated.");

            return View(model);
        }

        #region Password

        [HttpGet]
        public async Task<ActionResult> Password()
        {
            return await _userManager.HasPasswordAsync(User.Identity.GetUserId().ParseGuid())
                ? Redirect(Url.ChangePassword())
                : Redirect(Url.SetPassword());
        }

        public ActionResult ChangePassword()
        {
            ViewBag.ManageNavigationKey = "Password";

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewBag.ManageNavigationKey = "Password";

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.ChangePasswordAsync(_userContext.CurrentUser.Id, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ParseGuid());
                if (user != null)
                    await _signInManager.SignInAsync(user, false, false);

                AddSuccessMessage("Your password was successfully changed.");
            }
            else
                foreach (var error in result.Errors)
                    AddErrorMessage(error);

            return View(model);
        }

        public ActionResult SetPassword()
        {
            ViewBag.ManageNavigationKey = "Password";

            return View(new SetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            ViewBag.ManageNavigationKey = "Password";

            if (ModelState.IsValid)
            {
                var result = await _userManager.AddPasswordAsync(User.Identity.GetUserId().ParseGuid(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ParseGuid());
                    if (user != null)
                        await _signInManager.SignInAsync(user, false, false);

                    AddSuccessMessage("Your password has succesfully been set.");

                    return Redirect(Url.ChangePassword());
                }

                foreach (var error in result.Errors)
                    AddErrorMessage(error);
            }

            return View(model);
        }

        #endregion

        #region Logins

        public async Task<ActionResult> ManageLogins()
        {
            ViewBag.ManageNavigationKey = "Logins";

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ParseGuid());
            var userLogins = await _userManager.GetLoginsAsync(User.Identity.GetUserId().ParseGuid());
            var otherLogins = _authenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

            return View(new ManageLoginsViewModel
            {
                IsPasswordSet = !string.IsNullOrEmpty(user.PasswordHash),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            ViewBag.ManageNavigationKey = "Logins";

            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), _authenticationManager, User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync("XsrfId", User.Identity.GetUserId());

            if (loginInfo == null)
            {
                AddErrorMessage("An error has occurred.");
                return Redirect(Url.ManageLogins());
            }

            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId().ParseGuid(), loginInfo.Login);

            if (result.Succeeded)
            {
                var userName = loginInfo.ExternalIdentity.GetUserName();
                if (string.IsNullOrEmpty(userName))
                    userName = loginInfo.Login.ProviderKey;
                AddSuccessMessage("External login for " + userName + " was created.");
            }
            else
                foreach (var error in result.Errors)
                    AddErrorMessage(error);

            return Redirect(Url.ManageLogins());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            var result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId().ParseGuid(), new UserLoginInfo(loginProvider, providerKey));

            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ParseGuid());
                if (user != null)
                    await _signInManager.SignInAsync(user, false, false);

                AddSuccessMessage("The external login was successfully removed.");
            }
            else
                AddErrorMessage("An error has occurred.");

            return Redirect(Url.ManageLogins());
        }

        #endregion
    }
}