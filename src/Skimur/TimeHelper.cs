using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur
{
    public class TimeHelper
    {
        private const double Tolerance = 0.01;

        private static string IsPlural(int amount)
        {
            return (amount == 1 ? "" : "s");
        }

        public static string Age(TimeSpan timeSpan)
        {
            string result;

            if (timeSpan.TotalDays > 365)
            {
                //years
                double years = Math.Round(timeSpan.TotalDays / 365, 1);
                result = String.Format("{0} year{1}", years, (years > 1.0 ? "s" : ""));
            }
            else if (timeSpan.TotalDays > 31)
            {
                //months
                int months = (int)(timeSpan.TotalDays / 31);
                result = String.Format("{0} month{1}", months, IsPlural(months));
            }
            else if (timeSpan.TotalHours >= 24)
            {
                //days 
                result = String.Format("{0} day{1}", (int)timeSpan.TotalDays, IsPlural((int)timeSpan.TotalDays));
            }
            else if (timeSpan.TotalHours > 1 || Math.Abs(timeSpan.TotalHours - 1) < Tolerance)
            {
                //hours
                result = String.Format("{0} hour{1}", (int)timeSpan.TotalHours, IsPlural((int)timeSpan.TotalHours));
            }
            else if (timeSpan.TotalSeconds >= 60)
            {
                //minutes
                result = String.Format("{0} minute{1}", (int)timeSpan.TotalMinutes, IsPlural((int)timeSpan.TotalMinutes));
            }
            else
            {
                //seconds
                result = String.Format("{0} second{1}", (int)timeSpan.TotalSeconds, IsPlural((int)timeSpan.TotalSeconds));
            }

            return result;
        }
    }
}
