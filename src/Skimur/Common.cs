using System;

namespace Skimur
{
    public class Common
    {
        public static Func<DateTime> CurrentTime = () => DateTime.UtcNow;
    }
}
