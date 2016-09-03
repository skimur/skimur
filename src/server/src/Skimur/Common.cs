using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur
{
    public class Common
    {
        public static Func<DateTime> CurrentTime = () => DateTime.UtcNow;
    }
}
