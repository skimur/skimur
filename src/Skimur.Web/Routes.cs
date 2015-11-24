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
                name:"Subs",
                url:"subs",
                defaults:new {controller="Subs", action="Popular"});

            routes.MapRoute(
                name: "SubsNew",
                url: "subs/new",
                defaults: new { controller = "Subs", action = "New" });

            routes.MapRoute(
                name: "SubsSubscribed",
                url: "subs/subscribed",
                defaults: new {controller="Subs", action="Subscribed"});

            routes.MapRoute(
               name: "SubsModerating",
               url: "subs/moderating",
               defaults: new { controller = "Subs", action = "Moderating" });
            
            routes.MapRoute(
                name: "Frontpage",
                url: "",
                defaults: new { controller = "Posts", action = "Frontpage" });

            routes.MapRoute(
              name: "FrontpageHot",
              url: "hot",
              defaults: new { controller = "Posts", action = "Frontpage", sort = "hot" });

            routes.MapRoute(
               name: "FrontpageNew",
               url: "new",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "new" });

            routes.MapRoute(
               name: "FrontpageControversial",
               url: "controversial",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "controversial" });

            routes.MapRoute(
               name: "FrontpageTop",
               url: "top",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "top" });

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
               url: "s/{subName}",
               defaults: new { controller = "Posts", action = "Posts" });

            routes.MapRoute(
               name: "SubHot",
               url: "s/{subName}/hot",
               defaults: new { controller = "Posts", action = "Posts", sort = "hot" });

            routes.MapRoute(
               name: "SubNew",
               url: "s/{subName}/new",
               defaults: new { controller = "Posts", action = "Posts", sort = "new" });

            routes.MapRoute(
               name: "SubControversial",
               url: "s/{subName}/controversial",
               defaults: new { controller = "Posts", action = "Posts", sort = "controversial" });

            routes.MapRoute(
               name: "SubTop",
               url: "s/{subName}/top",
               defaults: new { controller = "Posts", action = "Posts", sort = "top" });

            routes.MapRoute(
               name: "SubSearch",
               url: "s/{subName}/search",
               defaults: new { controller = "Subs", action = "SearchSub" });

            routes.MapRoute(
                name: "SubEdit",
                url: "s/{subName}/edit",
                defaults: new {controller = "Subs", action = "Edit"});

            routes.MapRoute(
                name: "SubsUnmoderated",
                url: "s/{subName}/details/unmoderated",
                defaults: new { controller = "Posts", action = "Unmoderated" });

            routes.MapRoute(
                 name: "SubsReportedPosts",
                 url: "s/{subName}/reported/posts",
                 defaults: new { controller = "Reports", action = "ReportedPosts" });

            routes.MapRoute(
                name: "SubsReportedComments",
                url: "s/{subName}/reported/comments",
                defaults: new { controller = "Reports", action = "ReportedComments" });

            routes.MapRoute(
                name: "Post",
                url: "s/{subName}/post/{id}/{title}",
                defaults: new { controller = "Posts", action = "Post", title = UrlParameter.Optional });

            routes.MapRoute(
                name: "PostComment",
                url: "s/{subName}/post/{id}/{title}/c/{commentId}",
                defaults: new { controller = "Posts", action = "Post", title = UrlParameter.Optional });

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
                name: "SubModerators",
                url: "s/{subName}/moderators",
                defaults: new { controller = "Moderators", action = "Moderators" });

            routes.MapRoute(
                name: "SubStyles",
                url: "s/{subName}/styles",
                defaults: new {controller = "Styles", action = "Edit"});

            routes.MapRoute(
                name: "MoreComments",
                url: "morecomments",
                defaults: new { controller = "Comments", action = "More" });

            routes.MapRoute(
                name: "User",
                url: "user/{userName}",
                defaults: new { controller = "Users", action = "User" });

            routes.MapRoute(
                name: "UserComments",
                url: "user/{userName}/comments",
                defaults: new { controller = "Users", action = "Comments" });

            routes.MapRoute(
                name: "UserPosts",
                url: "user/{userName}/posts",
                defaults: new { controller = "Users", action = "Posts" });
            
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
                defaults: new { controller = "Comments", action = "Create" });

            routes.MapRoute(
               name: "EditComment",
               url: "editcomment",
               defaults: new { controller = "Comments", action = "Edit" });

            routes.MapRoute(
                name: "DeleteComment",
                url: "deletecomment",
                defaults: new { controller = "Comments", action = "Delete" });

            routes.MapRoute(
                name: "DeletePost",
                url: "deletepost",
                defaults: new { controller = "Posts", action = "Delete" });

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
                defaults: new { controller = "Posts", action = "Create" });

            routes.MapRoute(
                name: "SubmitWithSub",
                url: "s/{subName}/submit",
                defaults: new { controller = "Posts", action = "Create" });

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
                name: "MessageForSub",
                url: "s/{subName}/messages",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMail });

            routes.MapRoute(
                name: "MessageForSubUnread",
                url: "s/{subName}/messages/unread",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailUnread });

            routes.MapRoute(
              name: "MessageForSubSent",
              url: "s/{subName}/messages/sent",
              defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailSent });

            routes.MapRoute(
                name: "MessageForModerator",
                url: "messages/moderated",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMail });

            routes.MapRoute(
                name: "MessageForModeratorUnread",
                url: "messages/moderated/unread",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailUnread });

            routes.MapRoute(
                name: "MessageForModeratorSent",
                url: "messages/moderated/sent",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailSent });

            routes.MapRoute(
                name: "HttpError",
                url: "error",
                defaults: new { controller = "HttpStatus", action = "Error" });

            routes.MapRoute(
                name: "HttpNotFound",
                url: "notfound",
                defaults: new { controller = "HttpStatus", action = "NotFound" });

            routes.MapRoute(
                name: "HttpUnAuthorized",
                url: "unauthorized",
                defaults: new { controller = "HttpStatus", action = "UnAuthorized" });

            routes.MapRoute(
                name: "About",
                url: "about",
                defaults: new { controller = "Pages", action = "About" });

            routes.MapRoute(
                name: "EditPost",
                url: "editpost",
                defaults: new { controller = "Posts", action = "Edit" });

            routes.MapRoute(
                name: "Thumbnail",
                url: "thumbnail/{type}/{thumbnail}",
                defaults: new {controller="Thumbnail", action= "Thumbnail" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
