using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web
{
    public static class MessageUrls
    {
        public static string Compose(this UrlHelper urlHelper, string to = null, string subject = null, string body = null)
        {
            return urlHelper.RouteUrl("MessageCompose", new {to, subject, body});
        }

        public static string Inbox(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageInbox");
        }
    }
}
