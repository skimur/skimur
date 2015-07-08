using System.Web.Mvc;

// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace Skimur.Web
{
    public static class Urls
    {
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

        public static string Post(this UrlHelper urlHelper, string slug, string title = null)
        {
            return urlHelper.RouteUrl("Post", new { slug, title });
        }

        public static string User(this UrlHelper urlHelper, string userName)
        {
            return urlHelper.RouteUrl("User", new { userName });
        }

        public static string Domain(this UrlHelper urlHelper, string domain)
        {
            return urlHelper.RouteUrl("Domain", new { domain });
        }
    }
}
