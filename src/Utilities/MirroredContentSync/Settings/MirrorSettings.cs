using System.Collections.Generic;
using RedditSharp.Things;
using Skimur;

namespace MirroredContentSync.Settings
{
    public class MirrorSettings : ISettings
    {
        public MirrorSettings()
        {
            PostsPerSub = 2;
            FromTime = FromTime.Day;
            BotName = "skimur";
        }
        
        public int PostsPerSub { get; set; }

        public FromTime FromTime { get; set; }
        
        public List<string> SubsToMirror { get; set; } 

        public string BotName { get; set; }
    }
}
