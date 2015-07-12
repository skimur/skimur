using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    public class HotFactor
    {
        public static long GetHotFactor(int hot, long now, long timeStamp, double? ageweight = null)
        {
            if (ageweight == null)
                ageweight = 0.0;

            return (long)Math.Max(hot + ((now - timeStamp) * ageweight.Value) / 45000, 1.0);
        }
    }
}
