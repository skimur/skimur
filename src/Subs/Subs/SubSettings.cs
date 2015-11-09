using System;
using Skimur;

namespace Subs
{
    public class SubSettings : ISettings
    {
        public SubSettings()
        {
            MinUserAgeCreateSub = 0;
            ActivityExpirationSeconds = (int)TimeSpan.FromMinutes(15).TotalSeconds;
            MaximumNumberOfSubscribedSubs = 25;
            MaximumNumberOfModdedSubs = 10;
            MaximumNumberOfStickyPosts = 2;
        }

        public int MinUserAgeCreateSub { get; set; }

        public int ActivityExpirationSeconds { get; set; }

        public int MaximumNumberOfSubscribedSubs { get; set; }

        public int MaximumNumberOfModdedSubs { get; set; }

        public int MaximumNumberOfStickyPosts { get; set; }
    }
}
