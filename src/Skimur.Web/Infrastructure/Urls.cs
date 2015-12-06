using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Skimur.Web.Services;
using Subs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Infrastructure
{
    public static class Urls
    {
        public static string ModifyQuery(this IUrlHelper urlHelper, HttpContext context, string name, string value)
        {
            var url = context.Request.Url;
            if (url == null) throw new ArgumentNullException();

            return urlHelper.ModifyQuery(url.PathAndQuery, name, value);
        }

        public static string ModifyQuery(this IUrlHelper urlHelper, string url, string name, string value)
        {
            if (url == null) throw new ArgumentNullException();
            var queryStartIndex = url.IndexOf("?");

            if (queryStartIndex == -1)
                return url + "?" + name + "=" + value;

            var queries = HttpUtility.ParseQueryString(url.Substring(queryStartIndex));
            queries[name] = value;
            return url.Substring(0, queryStartIndex) + "?" + queries;
        }

        public static string Subs(this IUrlHelper urlHelper, string query = null)
        {
            return urlHelper.RouteUrl("Subs", new { query });
        }

        public static string SubsNew(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("SubsNew");
        }

        public static string SubsSubscribed(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("SubsSubscribed");
        }

        public static string SubsModerating(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("SubsModerating");
        }

        public static string Sub(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("Sub", new { subName });
        }

        public static string CreateSub(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "Subs");
        }

        public static string EditSub(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubEdit", new { subName });
        }

        public static string SubBans(this IUrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubBans", new { subName = name });
        }

        public static string SubBan(this IUrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubBan", new { subName = name });
        }

        public static string SubUnBan(this IUrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubUnBan", new { subName = name });
        }

        public static string SubUpdateBan(this IUrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubUpdateBan", new { subName = name });
        }

        public static string CreatePost(this IUrlHelper urlHelper, string subName = null)
        {
            if (!string.IsNullOrEmpty(subName))
                return urlHelper.RouteUrl("SubmitWithSub", new { subName });
            return urlHelper.RouteUrl("Submit");
        }

        public static string Post(this IUrlHelper urlHelper, string subName, Guid id, string title = null)
        {
            return urlHelper.RouteUrl("Post", new { subName, id, title = title.UrlFriendly() });
        }

        public static string Post(this IUrlHelper urlHelper, Sub sub, Post post)
        {
            return urlHelper.Post(sub.Name, post.Id, post.Title);
        }

        public static string Comment(this IUrlHelper urlHelper, string subName, Post post, Comment comment)
        {
            return urlHelper.RouteUrl("PostComment", new { subName, id = post.Id, title = post.Title.UrlFriendly(), commentId = comment.Id });
        }

        public static string User(this IUrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("User", new { userName });
        }

        public static string UserComments(this IUrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("UserComments", new { userName });
        }

        public static string UserPosts(this IUrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("UserPosts", new { userName });
        }

        public static string Domain(this IUrlHelper urlHelper, string domain)
        {
            return urlHelper.RouteUrl("Domain", new { domain });
        }

        public static string Search(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Search");
        }

        public static string SubSearch(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubSearch", new { subName });
        }

        public static string Login(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Login", "Account");
        }

        public static string Register(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Register", "Account");
        }

        public static string ForgotPassword(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("ForgotPassword", "Account");
        }

        public static string ManagePassword(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Password", "Manage");
        }

        public static string ChangePassword(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("ChangePassword", "Manage");
        }

        public static string SetPassword(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("SetPassword", "Manage");
        }

        public static string ManageProfile(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Index", "Manage");
        }

        public static string ManageEmail(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("ManageEmail", "Manage");
        }

        public static string ManageLogins(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("ManageLogins", "Manage");
        }

        public static string ManagePreferences(this IUrlHelper urlHelper)
        {
            return urlHelper.Action("Preferences", "Manage");
        }

        public static string AvatarUrl(this IUrlHelper urlHelper, string avatarIdentifier)
        {
            if (string.IsNullOrEmpty(avatarIdentifier))
                return urlHelper.Content("~/content/img/avatar.jpg");

            return urlHelper.Content("~/avatar/" + avatarIdentifier);
        }

        public static string Unmoderated(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsUnModerated", new { subName });
        }

        public static string ReportedPosts(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsReportedPosts", new { subName });
        }

        public static string ReportedComments(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsReportedComments", new { subName });
        }

        public static string Moderators(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubModerators", new { subName });
        }

        public static string Styles(this IUrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubStyles", new { subName });
        }

        public static string Thumbnail(this IUrlHelper urlHelper, string thumb, ThumbnailType type)
        {
            return urlHelper.RouteUrl("Thumbnail", new { thumbnail = thumb, type });
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName)
        {
            return urlHelper.RouteUrl(new UrlRouteContext { RouteName = routeName });
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, object values)
        {
            return urlHelper.RouteUrl(new UrlRouteContext { RouteName = routeName, Values = values });
        }
    }
}
