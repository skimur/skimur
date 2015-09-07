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
            return MessagesAll(urlHelper);
        }

        public static string MessagesAll(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageAll");
        }

        public static string MessagesUnread(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageUnread");
        }

        public static string MessagesPrivate(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessagePrivate");
        }

        public static string MessagesCommentReplies(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageCommentReplies");
        }

        public static string MessagesPostReplies(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessagePostReplies");
        }

        public static string MessagesMentions(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageMentions");
        }
    }
}
