using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web
{
    public static class HtmlHelpers
    {
        public static string Age(this HtmlHelper helper, DateTime dateTime)
        {
            return TimeHelper.Age(new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks));
        }
    }
}
