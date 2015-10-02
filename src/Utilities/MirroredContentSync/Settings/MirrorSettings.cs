using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using RedditSharp.Things;

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
