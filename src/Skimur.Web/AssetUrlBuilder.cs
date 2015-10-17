using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using Newtonsoft.Json;

namespace Skimur.Web
{
    public static class AssetUrlBuilder
    {
        private static bool? _isUsingStaticAssets;
        private static string _staticAssetsHost;
        private static string _version = typeof (AssetUrlBuilder).Assembly.GetName().Version.ToString();

        public static bool IsUsingStatisAssets
        {
            get
            {
                if (_isUsingStaticAssets.HasValue) return _isUsingStaticAssets.Value;
                var setting = WebConfigurationManager.AppSettings["UseStaticAssets"];
                _isUsingStaticAssets = !string.IsNullOrEmpty(setting) && JsonConvert.DeserializeObject<bool>(setting);
                return _isUsingStaticAssets.Value;
            }
        }

        public static string StaticAssetHost
        {
            get
            {
                if (!IsUsingStatisAssets) return null;
                if (string.IsNullOrEmpty(_staticAssetsHost))
                {
                    _staticAssetsHost = WebConfigurationManager.AppSettings["StaticAssetsHost"];
                    if(string.IsNullOrEmpty(_staticAssetsHost))
                        throw new Exception("When settings UseStaticAssets=true, you must provide a valid StaticAssetsHost");
                    if (_staticAssetsHost.EndsWith("/"))
                        _staticAssetsHost = _staticAssetsHost.Substring(0, _staticAssetsHost.Length - 1);
                }
                return _staticAssetsHost;
            }
        }

        public static void RenderScripts(this HtmlHelper htmlHelper)
        {
            if (!IsUsingStatisAssets)
                htmlHelper.ViewContext.Writer.Write(Scripts.Render("~/bundles/scripts"));
            else
                htmlHelper.ViewContext.Writer.Write("<script src=\"" + StaticAssetHost + "/Scripts/script.js?v=" + _version + "\"></script>");
        }

        public static void RenderStyles(this HtmlHelper htmlHelper)
        {
            if (!IsUsingStatisAssets)
                htmlHelper.ViewContext.Writer.Write(Styles.Render("~/bundles/styles"));
            else
                htmlHelper.ViewContext.Writer.Write("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + StaticAssetHost + "/Content/site.css?v=" + _version + "\">");
        }

        public static void RenderEditorScripts(this HtmlHelper htmlHelper)
        {
            if (!IsUsingStatisAssets)
                htmlHelper.ViewContext.Writer.Write("<script src=\"/Scripts/ace/ace.js?v=" + _version + "\"></script>");
            else
                htmlHelper.ViewContext.Writer.Write("<script src=\"" + StaticAssetHost + "/Scripts/ace/ace.js?v=" + _version + "\"></script>");
        }

        public static string AssetUrl(this UrlHelper urlHelper, string url)
        {
            if (!IsUsingStatisAssets)
                return url;
            return StaticAssetHost + url;
        }
    }
}
