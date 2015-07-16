using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    public class Hot
    {
        public static double GetHot(int hot, long timeStamp)
        {
            var order = Math.Log10(Math.Max(Math.Abs(hot), 1));

            int sign;

            if (hot > 0)
                sign = 1;
            else if (hot < 0)
                sign = -1;
            else
                sign = 0;

            var seconds = timeStamp - 1134028003;

            return Math.Round(sign * order + seconds / 4500.0, 7);
        }

        public static double GetHotFactor(int hot, long now, long timestamp, double? ageWeight)
        {
            if (ageWeight == null)
                ageWeight = 0;

            return Math.Max(hot + ((now - timestamp) * ageWeight.Value) / 45000.0, 1);
        }
    }
}
