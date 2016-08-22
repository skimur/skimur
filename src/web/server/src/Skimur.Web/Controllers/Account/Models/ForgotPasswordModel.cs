using System.ComponentModel.DataAnnotations;

namespace Skimur.Web.Controllers.Account.Models
{
    public class ForgotPasswordModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
