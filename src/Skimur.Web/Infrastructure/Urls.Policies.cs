using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public static class PolicyUrls
    {
        public static string PrivacyPolicy(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("PrivacyPolicy");
        }

        public static string UserAgreement(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("UserAgreement");
        }

        public static string ContentPolicy(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("ContentPolicy");
        }
    }
}
