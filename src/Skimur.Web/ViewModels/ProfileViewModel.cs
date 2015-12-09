using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
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
        public IFormFile AvatarFile { get; set; }
    }
}
