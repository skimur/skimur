using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Skimur.Web.Services;
using Subs;

// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace Skimur.Web
{
    public static class Urls
    {
        public static string ModifyQuery(this UrlHelper urlHelper, string name, string value)
        {
            var url = urlHelper.RequestContext.HttpContext.Request.Url;
            if (url == null) throw new ArgumentNullException();

            return urlHelper.ModifyQuery(url.PathAndQuery, name, value);
        }

        public static string ModifyQuery(this UrlHelper urlHelper, string url, string name, string value)
        {
            if (url == null) throw new ArgumentNullException();
            var queryStartIndex = url.IndexOf("?");

            if (queryStartIndex == -1)
                return url + "?" + name + "=" + value;

            var queries = HttpUtility.ParseQueryString(url.Substring(queryStartIndex));
            queries[name] = value;
            return url.Substring(0, queryStartIndex) + "?" + queries;
        }

        public static string Subs(this UrlHelper urlHelper, string query = null)
        {
            return urlHelper.RouteUrl("Subs");
        }

        public static string SubsNew(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("SubsNew");
        }

        public static string Sub(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("Sub", new { subName });
        }

        public static string CreateSub(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "Subs");
        }

        public static string EditSub(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubEdit", new {subName});
        }

        public static string SubBans(this UrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubBans", new { subName = name });
        }

        public static string SubBan(this UrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubBan", new { subName = name });
        }

        public static string SubUnBan(this UrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubUnBan", new { subName = name });
        }

        public static string SubUpdateBan(this UrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("SubUpdateBan", new { subName = name });
        }

        public static string CreatePost(this UrlHelper urlHelper, string subName = null)
        {
            if (!string.IsNullOrEmpty(subName))
                return urlHelper.RouteUrl("SubmitWithSub", new { subName });
            return urlHelper.RouteUrl("Submit");
        }

        public static string Post(this UrlHelper urlHelper, string subName, Guid id, string title = null)
        {
            return urlHelper.RouteUrl("Post", new { subName, id, title = title.UrlFriendly() });
        }

        public static string Post(this UrlHelper urlHelper, Sub sub, Post post)
        {
            return urlHelper.Post(sub.Name, post.Id, post.Title);
        }

        public static string Comment(this UrlHelper urlHelper, string subName, Post post, Comment comment)
        {
            return urlHelper.RouteUrl("PostComment", new { subName, id = post.Id, title = post.Title.UrlFriendly(), commentId = comment.Id });
        }

        public static string User(this UrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("User", new { userName });
        }

        public static string UserComments(this UrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("UserComments", new { userName });
        }

        public static string UserPosts(this UrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("UserPosts", new { userName });
        }

        public static string Domain(this UrlHelper urlHelper, string domain)
        {
            return urlHelper.RouteUrl("Domain", new { domain });
        }

        public static string Search(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Search");
        }

        public static string SubSearch(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubSearch", new { subName });
        }

        public static string Login(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Login", "Account");
        }

        public static string Register(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Register", "Account");
        }

        public static string ForgotPassword(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ForgotPassword", "Account");
        }

        public static string ManagePassword(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Password", "Manage");
        }

        public static string ChangePassword(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ChangePassword", "Manage");
        }

        public static string SetPassword(this UrlHelper urlHelper)
        {
            return urlHelper.Action("SetPassword", "Manage");
        }

        public static string ManageProfile(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Index", "Manage");
        }

        public static string ManageEmail(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ManageEmail", "Manage");
        }

        public static string ManageLogins(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ManageLogins", "Manage");
        }

        public static string ManagePreferences(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Preferences", "Manage");
        }

        public static string AvatarUrl(this UrlHelper urlHelper, string avatarIdentifier)
        {
            if (string.IsNullOrEmpty(avatarIdentifier))
                return urlHelper.Content("~/content/img/avatar.jpg");

            return urlHelper.Content("~/avatar/" + avatarIdentifier);
        }

        public static string Unmoderated(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsUnModerated", new { subName });
        }

        public static string ReportedPosts(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsReportedPosts", new { subName });
        }

        public static string ReportedComments(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubsReportedComments", new { subName });
        }

        public static string Moderators(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubModerators", new { subName });
        }

        public static string Styles(this UrlHelper urlHelper, string subName)
        {
            return urlHelper.RouteUrl("SubStyles", new { subName });
        }

        public static string Thumbnail(this UrlHelper urlHelper, string thumb, ThumbnailType type)
        {
            return urlHelper.RouteUrl("Thumbnail", new {thumbnail = thumb, type});
        }
    }
}
