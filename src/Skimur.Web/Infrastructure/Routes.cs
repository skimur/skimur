

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Skimur.Web.ViewModels;

namespace Skimur.Web.Infrastructure
{
    public static class Routes
    {
        public static object UrlParameter { get; private set; }

        public static void Register(IRouteBuilder routes)
        {
            routes.MapRouteShim(
                name: "Subs",
                url: "subs",
                defaults: new { controller = "Subs", action = "Popular" });

            routes.MapRouteShim(
                name: "SubsNew",
                url: "subs/new",
                defaults: new { controller = "Subs", action = "New" });

            routes.MapRouteShim(
                name: "SubsSubscribed",
                url: "subs/subscribed",
                defaults: new { controller = "Subs", action = "Subscribed" });

            routes.MapRouteShim(
               name: "SubsModerating",
               url: "subs/moderating",
               defaults: new { controller = "Subs", action = "Moderating" });

            routes.MapRouteShim(
                name: "Frontpage",
                url: "",
                defaults: new { controller = "Posts", action = "Frontpage" });

            routes.MapRouteShim(
              name: "FrontpageHot",
              url: "hot",
              defaults: new { controller = "Posts", action = "Frontpage", sort = "hot" });

            routes.MapRouteShim(
               name: "FrontpageNew",
               url: "new",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "new" });

            routes.MapRouteShim(
               name: "FrontpageControversial",
               url: "controversial",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "controversial" });

            routes.MapRouteShim(
               name: "FrontpageTop",
               url: "top",
               defaults: new { controller = "Posts", action = "Frontpage", sort = "top" });

            routes.MapRouteShim(
                name: "Search",
                url: "search",
                defaults: new { controller = "Subs", action = "SearchSite" });

            routes.MapRouteShim(
                name: "VotePost",
                url: "votepost",
                defaults: new { controller = "Subs", action = "VotePost" });

            routes.MapRouteShim(
                name: "UnVotePost",
                url: "unvotepost",
                defaults: new { controller = "Subs", action = "UnVotePost" });

            routes.MapRouteShim(
                name: "VoteComment",
                url: "votecomment",
                defaults: new { controller = "Subs", action = "VoteComment" });

            routes.MapRouteShim(
                name: "UnVoteComment",
                url: "unvotecomment",
                defaults: new { controller = "Subs", action = "UnVoteComment" });

            routes.MapRouteShim(
                name: "SubRandom",
                url: "s/random",
                defaults: new { controller = "Subs", action = "Random" });

            routes.MapRouteShim(
               name: "Sub",
               url: "s/{subName}",
               defaults: new { controller = "Posts", action = "Posts" });

            routes.MapRouteShim(
               name: "SubHot",
               url: "s/{subName}/hot",
               defaults: new { controller = "Posts", action = "Posts", sort = "hot" });

            routes.MapRouteShim(
               name: "SubNew",
               url: "s/{subName}/new",
               defaults: new { controller = "Posts", action = "Posts", sort = "new" });

            routes.MapRouteShim(
               name: "SubControversial",
               url: "s/{subName}/controversial",
               defaults: new { controller = "Posts", action = "Posts", sort = "controversial" });

            routes.MapRouteShim(
               name: "SubTop",
               url: "s/{subName}/top",
               defaults: new { controller = "Posts", action = "Posts", sort = "top" });

            routes.MapRouteShim(
               name: "SubSearch",
               url: "s/{subName}/search",
               defaults: new { controller = "Subs", action = "SearchSub" });

            routes.MapRouteShim(
                name: "SubEdit",
                url: "s/{subName}/edit",
                defaults: new { controller = "Subs", action = "Edit" });

            routes.MapRouteShim(
                name: "SubsUnmoderated",
                url: "s/{subName}/details/unmoderated",
                defaults: new { controller = "Posts", action = "Unmoderated" });

            routes.MapRouteShim(
                 name: "SubsReportedPosts",
                 url: "s/{subName}/reported/posts",
                 defaults: new { controller = "Reports", action = "ReportedPosts" });

            routes.MapRouteShim(
                name: "SubsReportedComments",
                url: "s/{subName}/reported/comments",
                defaults: new { controller = "Reports", action = "ReportedComments" });

            routes.MapRouteShim(
                name: "Post",
                url: "s/{subName}/post/{id}/{title?}",
                defaults: new { controller = "Posts", action = "Post" });

            routes.MapRouteShim(
                name: "PostComment",
                url: "s/{subName}/post/{id}/{title?}/c/{commentId}",
                defaults: new { controller = "Posts", action = "Post" });

            routes.MapRouteShim(
                name: "SubBans",
                url: "s/{subName}/bans",
                defaults: new { controller = "SubBans", action = "Bans" });

            routes.MapRouteShim(
                name: "SubBan",
                url: "s/{subName}/ban",
                defaults: new { controller = "SubBans", action = "Ban" });

            routes.MapRouteShim(
                name: "SubUnBan",
                url: "s/{subName}/unban",
                defaults: new { controller = "SubBans", action = "UnBan" });

            routes.MapRouteShim(
                name: "SubUpdateBan",
                url: "s/{subName}/updateban",
                defaults: new { controller = "SubBans", action = "UpdateBan" });

            routes.MapRouteShim(
                name: "SubModerators",
                url: "s/{subName}/moderators",
                defaults: new { controller = "Moderators", action = "Moderators" });

            routes.MapRouteShim(
                name: "SubStyles",
                url: "s/{subName}/styles",
                defaults: new { controller = "Styles", action = "Edit" });

            routes.MapRouteShim(
                name: "MoreComments",
                url: "morecomments",
                defaults: new { controller = "Comments", action = "More" });

            routes.MapRouteShim(
                name: "User",
                url: "user/{userName}",
                defaults: new { controller = "Users", action = "User" });

            routes.MapRouteShim(
                name: "UserComments",
                url: "user/{userName}/comments",
                defaults: new { controller = "Users", action = "Comments" });

            routes.MapRouteShim(
                name: "UserPosts",
                url: "user/{userName}/posts",
                defaults: new { controller = "Users", action = "Posts" });

            routes.MapRouteShim(
                name: "Domain",
                url: "domain/{domain}",
                defaults: new { controller = "Domains", action = "Domain" });

            routes.MapRouteShim(
                name: "Subscribe",
                url: "subscribe/{subName?}",
                defaults: new { controller = "Subs", action = "Subscribe"});

            routes.MapRouteShim(
                name: "UnSubscribe",
                url: "unsubscribe/{subName?}",
                defaults: new { controller = "Subs", action = "UnSubscribe" });

            routes.MapRouteShim(
                name: "CreateComment",
                url: "createcomment",
                defaults: new { controller = "Comments", action = "Create" });

            routes.MapRouteShim(
               name: "EditComment",
               url: "editcomment",
               defaults: new { controller = "Comments", action = "Edit" });

            routes.MapRouteShim(
                name: "DeleteComment",
                url: "deletecomment",
                defaults: new { controller = "Comments", action = "Delete" });

            routes.MapRouteShim(
                name: "DeletePost",
                url: "deletepost",
                defaults: new { controller = "Posts", action = "Delete" });

            routes.MapRouteShim(
                name: "Avatar",
                url: "avatar/{key}",
                defaults: new { controller = "Avatar", action = "Key" });

            routes.MapRouteShim(
                name: "PrivacyPolicy",
                url: "help/privacypolicy",
                defaults: new { controller = "Policies", action = "PrivacyPolicy" });

            routes.MapRouteShim(
                name: "UserAgreement",
                url: "help/useragreement",
                defaults: new { controller = "Policies", action = "UserAgreement" });

            routes.MapRouteShim(
                name: "ContentPolicy",
                url: "help/contentpolicy",
                defaults: new { controller = "Policies", action = "ContentPolicy" });

            routes.MapRouteShim(
                name: "Submit",
                url: "submit",
                defaults: new { controller = "Posts", action = "Create" });

            routes.MapRouteShim(
                name: "SubmitWithSub",
                url: "s/{subName}/submit",
                defaults: new { controller = "Posts", action = "Create" });

            routes.MapRouteShim(
                name: "MessageCompose",
                url: "messages/compose",
                defaults: new { controller = "Messages", action = "Compose" });

            routes.MapRouteShim(
                name: "MessageAll",
                url: "messages/inbox",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.All });

            routes.MapRouteShim(
                name: "MessageUnread",
                url: "messages/unread",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Unread });

            routes.MapRouteShim(
                name: "MessagePrivate",
                url: "messages/messages",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Messages });

            routes.MapRouteShim(
                name: "MessageCommentReplies",
                url: "messages/comments",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.CommentReplies });

            routes.MapRouteShim(
                name: "MessagePostReplies",
                url: "messages/posts",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.PostReplies });

            routes.MapRouteShim(
                name: "MessageMentions",
                url: "messages/mentions",
                defaults: new { controller = "Messages", action = "Inbox", type = InboxType.Mentions });

            routes.MapRouteShim(
                name: "MessageDetails",
                url: "messages/details/{id}",
                defaults: new { controller = "Messages", action = "Details" });

            routes.MapRouteShim(
                name: "MessageSent",
                url: "messages/sent",
                defaults: new { controller = "Messages", action = "Sent" });

            routes.MapRouteShim(
                name: "MessageForSub",
                url: "s/{subName}/messages",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMail });

            routes.MapRouteShim(
                name: "MessageForSubUnread",
                url: "s/{subName}/messages/unread",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailUnread });

            routes.MapRouteShim(
              name: "MessageForSubSent",
              url: "s/{subName}/messages/sent",
              defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailSent });

            routes.MapRouteShim(
                name: "MessageForModerator",
                url: "messages/moderated",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMail });

            routes.MapRouteShim(
                name: "MessageForModeratorUnread",
                url: "messages/moderated/unread",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailUnread });

            routes.MapRouteShim(
                name: "MessageForModeratorSent",
                url: "messages/moderated/sent",
                defaults: new { controller = "Messages", action = "ModeratorMail", type = InboxType.ModeratorMailSent });

            routes.MapRouteShim(
                name: "HttpError",
                url: "error",
                defaults: new { controller = "HttpStatus", action = "Error" });

            routes.MapRouteShim(
                name: "HttpNotFound",
                url: "notfound",
                defaults: new { controller = "HttpStatus", action = "NotFound" });

            routes.MapRouteShim(
                name: "HttpUnAuthorized",
                url: "unauthorized",
                defaults: new { controller = "HttpStatus", action = "UnAuthorized" });

            routes.MapRouteShim(
                name: "About",
                url: "about",
                defaults: new { controller = "Pages", action = "About" });

            routes.MapRouteShim(
                name: "EditPost",
                url: "editpost",
                defaults: new { controller = "Posts", action = "Edit" });

            routes.MapRouteShim(
                name: "Thumbnail",
                url: "thumbnail/{type}/{thumbnail}",
                defaults: new { controller = "Thumbnail", action = "Thumbnail" });

            routes.MapRoute(name: "areaRoute",
                template: "{area:exists}/{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRouteShim(
                name: "Default",
                url: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" }
                );
        }

        public static void MapRouteShim(this IRouteBuilder routes, string name, string url, object defaults)
        {
            routes.MapRoute(name, url, defaults);
        }
    }
}
