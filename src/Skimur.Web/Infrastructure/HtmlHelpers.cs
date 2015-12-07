using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Skimur.Web.ViewModels;
using Subs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public static class HtmlHelpers
    {
        public static string Age(this HtmlHelper helper, DateTime dateTime)
        {
            return TimeHelper.Age(new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks));
        }

        public static HtmlString Pager<T>(this IHtmlHelper helper, PagedList<T> pager, string baseUrl)
        {
            if (pager == null)
                return HtmlString.Empty;

            if (!pager.HasPreviousPage && !pager.HasNextPage)
                return HtmlString.Empty;

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

            return new HtmlString(sb.ToString());
        }

        public static HtmlString Pager<T>(this IHtmlHelper helper, PagedList<T> pager)
        {
            var url = ""; // TOOD:
            if (url == null) throw new ArgumentNullException();
            return helper.Pager(pager, /*TODO:url.PathAndQuery*/"");
        }

        public static string Age(this IHtmlHelper helper, DateTime dateTime)
        {
            return TimeHelper.Age(new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks));
        }

        public static HtmlString ErrorMessages(this IHtmlHelper helper)
        {
            var errorMessages = helper.ViewContext.TempData["ErrorMessages"] as List<string>;

            if (!helper.ViewData.ModelState.IsValid)
            {
                var modelErrors = helper.ViewData.ModelState[String.Empty];
                if (modelErrors != null && modelErrors.Errors.Count > 0)
                {
                    if (errorMessages == null)
                        errorMessages = new List<string>();
                    errorMessages.AddRange(modelErrors.Errors.Select(modelError => modelError.ErrorMessage));
                }
            }

            if (errorMessages == null || !errorMessages.Any()) return HtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-errors alert alert-danger\"><ul>");
            foreach (var errorMessage in errorMessages)
            {
                html.Append("<li>" + errorMessage + "</li>");
            }
            html.Append("</ul></div>");

            return new HtmlString(html.ToString());
        }

        public static HtmlString SuccessMessages(this IHtmlHelper helper)
        {
            var successMessages = helper.ViewContext.TempData["SuccessMessages"] as List<string>;

            if (successMessages == null)
                return HtmlString.Empty;

            if (!successMessages.Any()) return HtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-infos alert alert-info\"><ul>");
            foreach (var successMessage in successMessages)
            {
                html.Append("<li>" + successMessage + "</li>");
            }
            html.Append("</ul></div>");

            return new HtmlString(html.ToString());
        }
    }
}
