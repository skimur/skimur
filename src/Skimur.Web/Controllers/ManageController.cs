using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Skimur.Web.Services;
using Skimur.Web.ViewModels.Manage;
using Skimur.Web.Infrastructure;
using Skimur.Web.ViewModels;
using Microsoft.AspNet.Http;
using Skimur.App;
using Skimur.App.Services;

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IAvatarService _avatarService;
        private readonly IMembershipService _membershipService;

        public ManageController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IAvatarService avatarService,
            IMembershipService membershipService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _avatarService = avatarService;
            _membershipService = membershipService;
        }
        
        public async Task<ActionResult> Index()
        {
            ViewBag.ManageNavigationKey = "Profile";

            var user = await GetCurrentUserAsync();

            return View(new ProfileViewModel
            {
                AvatarIdentifier = user.AvatarIdentifier,
                FullName = user.FullName,
                Bio = user.Bio,
                Url = user.Url,
                Location = user.Location
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ProfileViewModel model, IEnumerable<IFormFile> files)
        {
            ViewBag.ManageNavigationKey = "Profile";

            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();

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

            _membershipService.UpdateUserProfile(user.Id, model.FullName, model.Bio, model.Url, model.Location);

            AddSuccessMessage("Your profile has been updated.");

            return View(model);
        }

        #region Preferences

        public async Task<ActionResult> Preferences()
        {
            ViewBag.ManageNavigationKey = "Preferences";

            var user = await GetCurrentUserAsync();

            return View(new UserPreferencesViewModel
            {
                ShowNsfw = user.ShowNsfw,
                EnableStyles = user.EnableStyles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Preferences(UserPreferencesViewModel model)
        {
            ViewBag.ManageNavigationKey = "Preferences";

            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();
            user.ShowNsfw = model.ShowNsfw;
            user.EnableStyles = model.EnableStyles;
            await _userManager.UpdateAsync(user);

            AddSuccessMessage("Your preferences have been updated.");

            return View(model);
        }

        #endregion

        #region External logins

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            ViewBag.ManageNavigationKey = "Logins";

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    AddSuccessMessage("The external login was removed.");
                }
                else
                {
                    AddErrorMessage("An error has occurred.");
                }
            }
            return RedirectToAction(nameof(ManageLogins));
        }

        #endregion

        #region Two factor authentication

        // TODO: Add two-factor authentication support

        //public IActionResult AddPhoneNumber()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var user = await GetCurrentUserAsync();
        //    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
        //    await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
        //    return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        //}

        //[HttpGet]
        //public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(await GetCurrentUserAsync(), phoneNumber);
        //    // Send an SMS to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            AddSuccessMessage("Your phone number was added.");
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    // If we got this far, something failed, redisplay the form
        //    ModelState.AddModelError(string.Empty, "Failed to verify phone number");
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> RemovePhoneNumber()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.SetPhoneNumberAsync(user, null);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            AddSuccessMessage("Your phone number was removed.");
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                AddErrorMessage(error.Description);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        AddErrorMessage("An error has occurred.");
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EnableTwoFactorAuthentication()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        await _userManager.SetTwoFactorEnabledAsync(user, true);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        _logger.LogInformation(1, "User enabled two-factor authentication.");
        //    }
        //    return RedirectToAction(nameof(Index), "Manage");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DisableTwoFactorAuthentication()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        await _userManager.SetTwoFactorEnabledAsync(user, false);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        _logger.LogInformation(2, "User disabled two-factor authentication.");
        //    }
        //    return RedirectToAction(nameof(Index), "Manage");
        //}

        #endregion

        #region Password

        [HttpGet]
        public async Task<ActionResult> Password()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            return await _userManager.HasPasswordAsync(user)
                ? RedirectToAction(nameof(ChangePassword))
                : RedirectToAction(nameof(SetPassword));
        }
        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.ManageNavigationKey = "Password";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewBag.ManageNavigationKey = "Password";

            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    AddSuccessMessage("Your password has been changed.");
                }
                else
                    foreach(var error in result.Errors)
                        AddErrorMessage(error.Description);
            }
            else
                AddErrorMessage("An error has occurred.");
            
            return View(model);
        }
        
        [HttpGet]
        public IActionResult SetPassword()
        {
            ViewBag.ManageNavigationKey = "Password";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            ViewBag.ManageNavigationKey = "Password";

            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    AddSuccessMessage("Your password has been set.");
                    return RedirectToAction(nameof(ChangePassword));
                }
                else
                    foreach (var error in result.Errors)
                        AddErrorMessage(error.Description);
            }
            else
                AddErrorMessage("An error has occurred.");
            
            return View(model);
        }

        #endregion

        #region Emails

        public async Task<ActionResult> ManageEmail()
        {
            ViewBag.ManageNavigationKey = "Email";

            var user = await GetCurrentUserAsync();

            var model = new ManageEmailViewModel();
            model.CurrentEmail = user.Email;
            model.IsCurrentEmailConfirmed = user.EmailConfirmed;
            model.IsPasswordSet = !string.IsNullOrEmpty(user.PasswordHash);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageEmail(ManageEmailViewModel model)
        {
            ViewBag.ManageNavigationKey = "Email";

            var user = await GetCurrentUserAsync();
            model.CurrentEmail = user.Email;
            model.IsCurrentEmailConfirmed = user.EmailConfirmed;
            model.IsPasswordSet = !string.IsNullOrEmpty(user.PasswordHash);

            if (!ModelState.IsValid)
                return View(model);

            if(!(await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                ModelState.AddModelError(string.Empty, "The provided password is invalid.");
                return View(model);
            }
            
            if (string.Equals(user.Email, model.NewEmail, StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The email provided is already set for this account.");
                return View(model);
            }

            user.Email = model.NewEmail;
            user.EmailConfirmed = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // send a confirmation email
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

                model.CurrentEmail = user.Email;
                model.IsCurrentEmailConfirmed = false;

                AddSuccessMessage("Your e-mail has been changed. A email has been sent to confirm the email address.");
                return View(model);
            }
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReSendEmailConfirmation()
        {
            var user = await GetCurrentUserAsync();

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

            AddSuccessMessage("A confirmation email has been sent.");

            return RedirectToAction("ManageEmail");
        }

        #endregion

        #region External logins

        [HttpGet]
        public async Task<IActionResult> ManageLogins()
        {
            ViewBag.ManageNavigationKey = "Logins";

            var user = await GetCurrentUserAsync();

            if (user == null)
                return View("Error");

            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();

            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                IsPasswordSet = !string.IsNullOrEmpty(user.PasswordHash)
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, User.GetUserId());
            return new ChallengeResult(provider, properties);
        }
        
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return View("Error");

            var info = await _signInManager.GetExternalLoginInfoAsync(User.GetUserId());

            if (info == null)
            {
                AddErrorMessage("An error has occurred.");
                return RedirectToAction(nameof(ManageLogins));
            }

            var result = await _userManager.AddLoginAsync(user, info);

            if (result.Succeeded)
                AddSuccessMessage("The external login was added.");
            else
                AddErrorMessage("An error has occurred.");

            return RedirectToAction(nameof(ManageLogins));
        }

        #endregion

        #region Helpers

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        #endregion
    }
}
