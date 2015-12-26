using System.ComponentModel.DataAnnotations;

namespace Skimur.Web.ViewModels
{
    public class UserPreferencesViewModel
    {
        [Display(Name="I am over eighteen years old and willing to view adult content")]
        public bool ShowNsfw { get; set; }

        [Display(Name ="Allow subs to present custom styles to me.")]
        public bool EnableStyles { get; set; }
    }
}
