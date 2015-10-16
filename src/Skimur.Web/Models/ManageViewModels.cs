using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Skimur.Web.Models
{
    public class ProfileViewModel
    {
        [DisplayName("Full name")]
        [MaxLength(50, ErrorMessage = "Full name name cannot be more than 50 characters.")]
        public string FullName { get; set; }

        [DisplayName("Bio")]
        [MaxLength(150, ErrorMessage = "Bio cannot be more than 150 characters.")]
        public string Bio { get; set; }

        [DisplayName("Url")]
        public string Url { get; set; }

        [DisplayName("Location")]
        [MaxLength(75, ErrorMessage = "Location cannot be more than 75 characters.")]
        public string Location { get; set; }

        [Display(Name = "Avatar")]
        public string AvatarIdentifier { get; set; }

        [Display(Name = "Avatar")]
        public HttpPostedFileBase AvatarFile { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public bool IsPasswordSet { get; set; }
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class ManagePasswordViewModel
    {
        
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ManageEmailViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        
        public string CurrentEmail { get; set; }

        public bool IsCurrentEmailConfirmed { get; set; }

        public bool IsPasswordSet { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("New email")]
        public string NewEmail { get; set; }

        [DisplayName("Confirm new email")]
        [Compare("NewEmail", ErrorMessage = "The new password and confirmation password do not match.")]
        public string NewEmailConfirmed { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class UserPreferencesModel
    {
        [DisplayName("I am over eighteen years old and willing to view adult content")]
        public bool ShowNsfw { get; set; }

        [DisplayName("Allow subs to present custom styles to me.")]
        public bool EnableStyles { get; set; }
    }
}