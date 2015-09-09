using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Subs;

namespace Skimur.Web
{
    public static class MessageUrls
    {
        public static string Compose(this UrlHelper urlHelper, string to = null, string subject = null, string body = null)
        {
            return urlHelper.RouteUrl("MessageCompose", new { to, subject, body });
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

        public static string MessageDetails(this UrlHelper urlHelper, Guid messageId, Guid? context = null)
        {
            return urlHelper.RouteUrl("MessageDetails", new { id = messageId, context });
        }

        public static string MessageDetails(this UrlHelper urlHelper, Message message)
        {
            return message.FirstMessage.HasValue ? MessageDetails(urlHelper, message.FirstMessage.Value, message.Id) : MessageDetails(urlHelper, message.Id);
        }

        public static string Sent(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageSent");
        }

        public static string MessagesForSub(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("MessageForSub", new { subName });
        }

        public static string MessagesForSubUnread(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("MessageForSubUnRead", new { subName });
        }


        public static string MessagesForModeratedSubs(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageForModerator");
        }

        public static string MessagesForModeratedSubsUnread(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageForModeratorUnread");
        }
    }
}
