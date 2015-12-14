using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Skimur.Web.ViewModels;
using Subs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
                sb.Append(string.Format("<a class=\"btn btn-default\" href=\"{0}\">{1}</a>", Urls.ModifyQuery(baseUrl, "pageNumber", (pager.PageNumber - 1).ToString()), "Previous"));
                if (pager.HasNextPage)
                {
                    sb.Append("&nbsp;&nbsp;");
                }
            }

            if (pager.HasNextPage)
            {
                sb.Append(string.Format("<a class=\"btn btn-default\" href=\"{0}\">{1}</a>", Urls.ModifyQuery(baseUrl, "pageNumber", (pager.PageNumber + 1).ToString()), "Next"));
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
            List<string> messages = null;
            
            var errorMessagesArray = helper.ViewContext.TempData["ErrorMessages"] as string[];

            if(errorMessagesArray != null && errorMessagesArray.Length > 0)
            {
                if (messages == null)
                    messages = new List<string>();
                foreach (var message in errorMessagesArray)
                    messages.Add(message);
            }

            var errorMessagesList = helper.ViewContext.TempData["ErrorMessages"] as List<string>;

            if (errorMessagesList != null && errorMessagesList.Count > 0)
            {
                if (messages == null)
                    messages = new List<string>();
                foreach (var message in errorMessagesList)
                    messages.Add(message);
            }

            if (!helper.ViewData.ModelState.IsValid)
            {
                var modelErrors = helper.ViewData.ModelState[string.Empty];
                if (modelErrors != null && modelErrors.Errors.Count > 0)
                {
                    if (messages == null)
                        messages = new List<string>();
                    messages.AddRange(modelErrors.Errors.Select(modelError => modelError.ErrorMessage));
                }
            }

            if (messages == null || messages.Count == 0) return HtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-errors alert alert-danger\"><ul>");
            foreach (var message in messages)
            {
                html.Append("<li>" + message + "</li>");
            }
            html.Append("</ul></div>");

            return new HtmlString(html.ToString());
        }

        public static HtmlString SuccessMessages(this IHtmlHelper helper)
        {
            List<string> messages = null;

            var successMessagesArray = helper.ViewContext.TempData["SuccessMessages"] as string[];

            if(successMessagesArray != null && successMessagesArray.Length > 0)
            {
                if (messages == null)
                    messages = new List<string>();
                foreach(var message in successMessagesArray)
                    messages.Add(message);
            }

            var successMessagesList = helper.ViewContext.TempData["SuccessMessages"] as List<string>;

            if (successMessagesList != null && successMessagesList.Count > 0)
            {
                if (messages == null)
                    messages = new List<string>();
                foreach (var message in successMessagesList)
                    messages.Add(message);
            }

            if (messages == null)
                return HtmlString.Empty;

            if (messages.Count == 0) return HtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-infos alert alert-info\"><ul>");
            foreach (var message in messages)
            {
                html.Append("<li>" + message + "</li>");
            }
            html.Append("</ul></div>");

            return new HtmlString(html.ToString());
        }

        public static IList<SelectListItem> ItemsForEnum<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);

            var enumType = metadata.ModelType;
            var isNullable = false;

            {
                var underlyingType = Nullable.GetUnderlyingType(enumType);
                if (underlyingType != null)
                {
                    isNullable = true;
                    enumType = underlyingType;
                }
            }

            var items = Enum.GetValues(typeof(TProperty)).Cast<TProperty>().Select(x => new SelectListItem
            {
                Text = GetEnumDescription(x),
                Value = x.ToString(),
                Selected = x.Equals(metadata.Model)
            }).ToList();

            if (isNullable)
                items.Insert(0, new SelectListItem { Text = "", Value = "", Selected = metadata.Model == null });

            return items;
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
