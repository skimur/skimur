using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
{
    public class ManageEmailViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string CurrentEmail { get; set; }

        public bool IsCurrentEmailConfirmed { get; set; }

        public bool IsPasswordSet { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }

        [Display(Name = "Confirm new email")]
        [Compare("NewEmail", ErrorMessage = "The new password and confirmation password do not match.")]
        public string NewEmailConfirmed { get; set; }
    }
}
