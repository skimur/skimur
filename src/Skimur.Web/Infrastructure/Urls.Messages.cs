using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Subs;
using System;

namespace Skimur.Web.Infrastructure
{
    public static class MessageUrls
    {
        public static string Compose(this IUrlHelper urlHelper, string to = null, string subject = null, string body = null)
        {
            return urlHelper.RouteUrl("MessageCompose", new { to, subject, body });
        }

        public static string Inbox(this IUrlHelper urlHelper)
        {
            return MessagesAll(urlHelper);
        }

        public static string MessagesAll(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageAll");
        }

        public static string MessagesUnread(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageUnread");
        }

        public static string MessagesPrivate(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessagePrivate");
        }

        public static string MessagesCommentReplies(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageCommentReplies");
        }

        public static string MessagesPostReplies(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessagePostReplies");
        }

        public static string MessagesMentions(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageMentions");
        }

        public static string MessageDetails(this IUrlHelper urlHelper, Guid messageId, Guid? context = null)
        {
            return urlHelper.RouteUrl("MessageDetails", new { id = messageId, context });
        }

        public static string MessageDetails(this IUrlHelper urlHelper, Message message)
        {
            return message.FirstMessage.HasValue ? MessageDetails(urlHelper, message.FirstMessage.Value, message.Id) : MessageDetails(urlHelper, message.Id);
        }

        public static string Sent(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageSent");
        }

        public static string MessagesForSub(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("MessageForSub", new { subName });
        }

        public static string MessagesForSubUnread(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("MessageForSubUnread", new { subName });
        }

        public static string MessagesForSubSent(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("MessageForSubSent", new { subName });
        }
        
        public static string MessagesForModeratedSubs(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageForModerator");
        }

        public static string MessagesForModeratedSubsUnread(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageForModeratorUnread");
        }

        public static string MessagesForModeratedSubsSent(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("MessageForModeratorSent");
        }
    }
}
