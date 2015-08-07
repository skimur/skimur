using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
            return urlHelper.Action("Index", "Subs", new { query });
        }

        public static string Sub(this UrlHelper urlHelper, string name)
        {
            return urlHelper.RouteUrl("Sub", new { name });
        }

        public static string EditSub(this UrlHelper urlHelper, string name)
        {
            return urlHelper.Action("Edit", "Subs", new { id = name });
        }

        public static string CreatePost(this UrlHelper urlHelper)
        {
            return urlHelper.Action("CreatePost", "Subs");
        }

        public static string Post(this UrlHelper urlHelper, string subName, string slug, string title = null)
        {
            return urlHelper.RouteUrl("Post", new { subName, slug, title = title.UrlFriendly() });
        }

        public static string Post(this UrlHelper urlHelper, Post post)
        {
            return urlHelper.Post(post.SubName, post.Slug, post.Title);
        }

        public static string Comment(this UrlHelper urlHelper, string subName, Post post, Comment comment)
        {
            return urlHelper.RouteUrl("PostComment", new { subName, slug = post.Slug, title=post.Title.UrlFriendly(), commentId = comment.Id });
        }

        public static string User(this UrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("User", new { userName });
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
            return urlHelper.RouteUrl("SubSearch", new { name = subName });
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

        public static string ManageLogins(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ManageLogins", "Manage");
        }

        public static string AvatarUrl(this UrlHelper urlHelper, string avatarIdentifier)
        {
            if (string.IsNullOrEmpty(avatarIdentifier))
                return urlHelper.Content("~/content/img/avatar.jpg");

            return urlHelper.Content("~/avatar/" + avatarIdentifier);
        }
    }
}
