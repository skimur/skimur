using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Skimur.Web.Models;

namespace Skimur.Web
{
    public static class HtmlHelpers
    {
        public static string Age(this HtmlHelper helper, DateTime dateTime)
        {
            return TimeHelper.Age(new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks));
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pager, string baseUrl)
        {
            if(pager == null)
                return MvcHtmlString.Empty;

            if(!pager.HasPreviousPage && !pager.HasNextPage)
                return MvcHtmlString.Empty;

            var sb = new StringBuilder();

            if (pager.HasPreviousPage)
            {
                sb.Append(string.Format("<a class=\"btn btn-default\" href=\"{0}\">{1}</a>", Urls.ModifyQuery(null, baseUrl, "pageNumber", (pager.PageNumber - 1).ToString()), "Previous"));
                if (pager.HasNextPage)
                {
                    sb.Append("&nbsp;&nbsp;");
                }
            }

            if (pager.HasNextPage)
            {
                sb.Append(string.Format("<a class=\"btn btn-default\" href=\"{0}\">{1}</a>", Urls.ModifyQuery(null, baseUrl, "pageNumber", (pager.PageNumber + 1).ToString()), "Next"));
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagedList<T> pager)
        {
            var url = helper.ViewContext.RequestContext.HttpContext.Request.Url;
            if(url == null) throw new ArgumentNullException();
            return helper.Pager(pager, url.PathAndQuery);
        }
    }
}
