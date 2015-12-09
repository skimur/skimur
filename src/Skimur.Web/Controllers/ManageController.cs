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
using Membership.Services;
using Microsoft.AspNet.Http;

namespace Skimur.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly UserManager<Membership.User> _userManager;
        private readonly SignInManager<Membership.User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IAvatarService _avatarService;
        private readonly IMembershipService _membershipService;

        public ManageController(
            UserManager<Membership.User> userManager,
            SignInManager<Membership.User> signInManager,
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

        [ActionName("Index")]
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

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
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

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(1, "User enabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(2, "User disabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(await GetCurrentUserAsync(), phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    AddSuccessMessage("Your phone number was added.");
                    return RedirectToAction(nameof(Index));
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        [HttpGet]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    AddSuccessMessage("Your phone number was removed.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        AddErrorMessage(error.Description);
                    }
                }
            }
            else
            {
                AddErrorMessage("An error has occurred.");
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<ActionResult> Password()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            return await _userManager.HasPasswordAsync(user)
                ? RedirectToAction(nameof(ChangePassword))
                : RedirectToAction(nameof(SetPassword));
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
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
                {
                    foreach(var error in result.Errors)
                        AddErrorMessage(error.Description);
                }
            }
            else
            {
                AddErrorMessage("An error has occurred.");
            }
            
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
                {
                    foreach (var error in result.Errors)
                    {
                        AddErrorMessage(error.Description);
                    }
                }
            }
            else
            {
                AddErrorMessage("An error has occurred.");
            }
            
            return View(model);
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, User.GetUserId());
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(User.GetUserId());
            if (info == null)
            {
                AddErrorMessage("An error has occurred.");
                return RedirectToAction(nameof(ManageLogins));
            }
            var result = await _userManager.AddLoginAsync(user, info);

            if (result.Succeeded)
            {
                AddSuccessMessage("The external login was added.");
            }
            else
            {
                AddErrorMessage("An error has occurred.");
            }

            return RedirectToAction(nameof(ManageLogins));
        }

        #region Helpers
        
        private async Task<Membership.User> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        #endregion
    }
}
