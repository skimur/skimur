using System;
using Infrastructure.Settings;

namespace Subs
{
    public class SubSettings : ISettings
    {
        public SubSettings()
        {
            MinUserAgeCreateSub = 0;
            ActivityExpirationSeconds = (int)TimeSpan.FromMinutes(15).TotalSeconds;
        }

        public int MinUserAgeCreateSub { get; set; }

        public int ActivityExpirationSeconds { get; set; }
    }
}
