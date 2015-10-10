using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Skimur.Web.Models;
using Subs;

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
            if (pager == null)
                return MvcHtmlString.Empty;

            if (!pager.HasPreviousPage && !pager.HasNextPage)
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
            if (url == null) throw new ArgumentNullException();
            return helper.Pager(pager, url.PathAndQuery);
        }

        public static MvcHtmlString ErrorMessages(this HtmlHelper helper)
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

            if (errorMessages == null || !errorMessages.Any()) return MvcHtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-errors alert alert-danger\"><ul>");
            foreach (var errorMessage in errorMessages)
            {
                html.Append("<li>" + errorMessage + "</li>");
            }
            html.Append("</ul></div>");

            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString SuccessMessages(this HtmlHelper helper)
        {
            var successMessages = helper.ViewContext.TempData["SuccessMessages"] as List<string>;

            if (successMessages == null)
                return MvcHtmlString.Empty;

            if (!successMessages.Any()) return MvcHtmlString.Empty;

            var html = new StringBuilder();
            html.Append("<div class=\"validation-summary-infos alert alert-info\"><ul>");
            foreach (var successMessage in successMessages)
            {
                html.Append("<li>" + successMessage + "</li>");
            }
            html.Append("</ul></div>");

            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString ClientValidationErrorsContainer(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"validation-summary-valid text-danger\" data-valmsg-summary=\"true\"><ul><li style=\"display:none\"></li></ul></div>");
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
        
        public static IEnumerable<SelectListItem> ItemsForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

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

            var items = Enum.GetValues(enumType).Cast<TProperty>().Select(x => new SelectListItem
            {
                Text = GetEnumDescription(x),
                Value = x.ToString(),
                Selected = x.Equals(metadata.Model)
            }).ToList();

            if (isNullable)
                items.Insert(0, new SelectListItem { Text = "", Value = "", Selected = metadata.Model == null });

            return items;
        }

        public static void RenderSideBar(this HtmlHelper htmlHelper, string subName = null, Guid? subId = null, bool showSearch = true, bool showCreateSub = true, bool showSubmit = true)
        {
            htmlHelper.RenderAction("SideBar", "Common", new
            {
                subName,
                subId,
                showSearch,
                showCreateSub,
                showSubmit
            });
        }

        public static string ModeratorPermissionsText(this HtmlHelper htmlHelper, ModeratorPermissions permissions)
        {
            if (permissions.HasPermission(ModeratorPermissions.All))
                return "Full";

            if (permissions == ModeratorPermissions.None)
                return "None";

            var list = new List<string>();

            if (permissions.HasPermission(ModeratorPermissions.Access))
                list.Add("Access");
            if (permissions.HasPermission(ModeratorPermissions.Config))
                list.Add("Config");
            if (permissions.HasPermission(ModeratorPermissions.Flair))
                list.Add("Flair");
            if (permissions.HasPermission(ModeratorPermissions.Mail))
                list.Add("Mail");
            if (permissions.HasPermission(ModeratorPermissions.Posts))
                list.Add("Posts");

            return string.Join(", " , list);
        }
    }
}
