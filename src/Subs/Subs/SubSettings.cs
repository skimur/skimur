using Infrastructure.Settings;

namespace Subs
{
    public class SubSettings : ISettings
    {
        public SubSettings()
        {
            MinUserAgeCreateSub = 30;
        }

        public int MinUserAgeCreateSub { get; set; }
    }
}
