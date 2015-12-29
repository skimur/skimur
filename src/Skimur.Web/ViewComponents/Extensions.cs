using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using System;

namespace Skimur.Web.ViewComponents
{
    public static class Extensions
    {
        public static HtmlString SideBar(this IViewComponentHelper helper, string subName = null, Guid? subId = null, bool showSearch = true, bool showCreateSub = true, bool showSubmit = true)
        {
            return helper.Invoke("SideBar", subName, subId, showSearch, showCreateSub, showSubmit);
        }
    }
}
