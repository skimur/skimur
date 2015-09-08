using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Skimur.Web.Models;

namespace Skimur.Web
{
    public static class Routes
    {
        public static void Register(RouteCollection routes)
        {
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Frontpage",
                url: "",
                defaults: new { controller = "Subs", action = "Frontpage" });

            routes.MapRoute(
              name: "FrontpageHot",
              url: "hot",
              defaults: new { controller = "Subs", action = "Frontpage", sort = "hot" });

            routes.MapRoute(
               name: "FrontpageNew",
               url: "new",
               defaults: new { controller = "Subs", action = "Frontpage", sort = "new" });

            routes.MapRoute(
               name: "FrontpageControversial",
               url: "controversial",
               defaults: new { controller = "Subs", action = "Frontpage", sort = "controversial" });

            routes.MapRoute(
               name: "FrontpageTop",
               url: "top",
               defaults: new { controller = "Subs", action = "Frontpage", sort = "top" });

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Subs", action = "SearchSite" });

            routes.MapRoute(
                name: "VotePost",
                url: "votepost",
                defaults: new { controller = "Subs", action = "VotePost" });

            routes.MapRoute(
                name: "UnVotePost",
                url: "unvotepost",
                defaults: new { controller = "Subs", action = "UnVotePost" });

            routes.MapRoute(
                name: "VoteComment",
                url: "votecomment",
                defaults: new { controller = "Subs", action = "VoteComment" });

            routes.MapRoute(
                name: "UnVoteComment",
                url: "unvotecomment",
                defaults: new { controller = "Subs", action = "UnVoteComment" });

            routes.MapRoute(
                name: "SubRandom",
                url: "s/random",
                defaults: new { controller = "Subs", action = "Random" });

            routes.MapRoute(
               name: "Sub",
               url: "s/{name}",
               defaults: new { controller = "Subs", action = "Posts" });

            routes.MapRoute(
               name: "SubHot",
               url: "s/{name}/hot",
               defaults: new { controller = "Subs", action = "Posts", sort = "hot" });

            routes.MapRoute(
               name: "SubNew",
               url: "s/{name}/new",
               defaults: new { controller = "Subs", action = "Posts", sort = "new" });

            routes.MapRoute(
               name: "SubControversial",
               url: "s/{name}/controversial",
               defaults: new { controller = "Subs", action = "Posts", sort = "controversial" });

            routes.MapRoute(
               name: "SubTop",
               url: "s/{name}/top",
               defaults: new { controller = "Subs", action = "Posts", sort = "top" });

            routes.MapRoute(
               name: "SubSearch",
               url: "s/{name}/search",
               defaults: new { controller = "Subs", action = "SearchSub" });

            routes.MapRoute(
                name: "SubsUnmoderated",
                url: "s/{subName}/details/unmoderated",
                defaults: new { controller = "Posts", action = "Unmoderated" });

            routes.MapRoute(
                name: "Post",
                url: "s/{subName}/post/{id}/{title}",
                defaults: new { controller = "Subs", action = "Post", title = UrlParameter.Optional });

            routes.MapRoute(
                name: "PostComment",
                url: "s/{subName}/post/{id}/{title}/c/{commentId}",
                defaults: new { controller = "Subs", action = "Post", title = UrlParameter.Optional });

            routes.MapRoute(
                name: "SubBans",
                url: "s/{subName}/bans",
                defaults: new { controller = "SubBans", action = "Bans" });

            routes.MapRoute(
                name: "SubBan",
                url: "s/{subName}/ban",
                defaults: new { controller = "SubBans", action = "Ban" });

            routes.MapRoute(
                name: "SubUnBan",
                url: "s/{subName}/unban",
                defaults: new { controller = "SubBans", action = "UnBan" });

            routes.MapRoute(
                name: "SubUpdateBan",
                url: "s/{subName}/updateban",
                defaults: new { controller = "SubBans", action = "UpdateBan" });

            routes.MapRoute(
                name: "MoreComments",
                url: "morecomments",
                defaults: new { controller = "Subs", action = "MoreComments" });

            routes.MapRoute(
                name: "User",
                url: "user/{userName}",
                defaults: new { controller = "Users", action = "User" });

            routes.MapRoute(
                name: "Domain",
                url: "domain/{domain}",
                defaults: new { controller = "Domains", action = "Domain" });

            routes.MapRoute(
                name: "Subscribe",
                url: "subscribe/{subName}",
                defaults: new { controller = "Subs", action = "Subscribe", subName = UrlParameter.Optional /*not really optionally, but they could provide subName via ajax if they wish*/});

            routes.MapRoute(
                name: "UnSubscribe",
                url: "unsubscribe/{subName}",
                defaults: new { controller = "Subs", action = "UnSubscribe", subName = UrlParameter.Optional /*not really optionally, but they could provide subName via ajax if they wish*/ });

            routes.MapRoute(
                name: "CreateComment",
                url: "createcomment",
                defaults: new { controller = "Subs", action = "CreateComment" });

            routes.MapRoute(
               name: "EditComment",
               url: "editcomment",
               defaults: new { controller = "Subs", action = "EditComment" });

            routes.MapRoute(
                name: "DeleteComment",
                url: "deletecomment",
                defaults: new { controller = "Subs", action = "DeleteComment" });

            routes.MapRoute(
                name: "Avatar",
                url: "avatar/{key}",
                defaults: new { controller = "Avatar", action = "Key" });

            routes.MapRoute(
                name: "PrivacyPolicy",
                url: "help/privacypolicy",
                defaults: new { controller = "Policies", action = "PrivacyPolicy" });

            routes.MapRoute(
                name: "UserAgreement",
                url: "help/useragreement",
                defaults: new { controller = "Policies", action = "UserAgreement" });

            routes.MapRoute(
                name: "ContentPolicy",
                url: "help/contentpolicy",
                defaults: new { controller = "Policies", action = "ContentPolicy" });

            routes.MapRoute(
                name: "Submit",
                url: "submit",
                defaults: new { controller = "Subs", action = "CreatePost" });

            routes.MapRoute(
                name: "SubmitWithSub",
                url: "s/{subName}/submit",
                defaults: new { controller = "Subs", action = "CreatePost" });

            routes.MapRoute(
                name: "MessageCompose",
                url: "messages/compose",
                defaults: new { controller = "Messages", action = "Compose" });

            routes.MapRoute(
                name: "MessageAll",
                url: "messages/inbox",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.All });

            routes.MapRoute(
                name: "MessageUnread",
                url: "messages/unread",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Unread });

            routes.MapRoute(
                name: "MessagePrivate",
                url: "messages/messages",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Messages });

            routes.MapRoute(
                name: "MessageCommentReplies",
                url: "messages/comments",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.CommentReplies });

            routes.MapRoute(
                name: "MessagePostReplies",
                url: "messages/posts",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.PostReplies });

            routes.MapRoute(
                name: "MessageMentions",
                url: "messages/mentions",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Mentions });

            routes.MapRoute(
                name: "MessageDetails",
                url: "messages/details/{id}",
                defaults: new { controller = "Messages", action = "Details" });

            routes.MapRoute(
                name: "MessageSent",
                url: "messages/sent",
                defaults: new { controller = "Messages", action = "Sent" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
