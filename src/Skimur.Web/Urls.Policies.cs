using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web
{
    public static class PolicyUrls
    {
        public static string PrivacyPolicy(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("PrivacyPolicy");
        }

        public static string UserAgreement(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("UserAgreement");
        }

        public static string ContentPolicy(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("ContentPolicy");
        }
    }
}
